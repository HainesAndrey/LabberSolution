using LabberClient.VMStuff;
using LabberClient.Workspace.JournalsTab.JournalsSelector;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LabberClient.Workspace.DebtsTab
{
    public class DebtsTabPageVM : LabberVMBase
    {
        private List<Group> groups;
        private bool tableEnabled = true;
        private DataTable dataTable;

        //public JournalsSelectorPage JournalsSelector { get; set; }
        public List<Group> Groups { get => groups; set { groups = value; RaisePropertyChanged("Groups"); } }
        public List<Student> Students { get; set; }
        public List<Subject> Subjects { get; set; }
        public List<Mark> Marks { get; set; }
        public bool TableEnabled { get => tableEnabled; set { tableEnabled = value; RaisePropertyChanged("TableEnabled"); } }
        public Group CurrentGroup { get; set; }

        public delegate void UpdateTableEventHandler(DataTable table);
        public event UpdateTableEventHandler UpdateTable;

        public DebtsTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            //JournalsSelector = new JournalsSelectorPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
        }

        public override async void LoadData()
        {
            await Refresh();
            UpdateTable?.Invoke(dataTable);
        }

        private Task Refresh()
        {
            return Task.Run(() =>
            {
                using (db = new DBWorker())
                {
                    Groups = db.Groups.ToList().OrderBy(x => x.Title).ToList();
                }
                CurrentGroup = Groups.FirstOrDefault();
                if (CurrentGroup == null)
                    return;

                List<Journal_Lab> prev_journal_labs;
                using (db = new DBWorker())
                {
                    Students = db.Students.Where(x => x.GroupId == CurrentGroup.Id).ToList()
                        .OrderBy(x => x.Surname).ThenBy(x => x.FirstName).ThenBy(x => x.SecondName).ToList();

                    Subjects = db.Journals.Include(x => x.Subject).Where(x => x.GroupId == CurrentGroup.Id)
                        .Select(x => x.Subject).Distinct().OrderBy(x => x.ShortTitle).ToList();

                    var journalsids = db.Journals.Where(x => x.GroupId == CurrentGroup.Id).Select(x => x.Id).ToList();

                    var journal_labsids = new List<uint>();
                    foreach (var item in journalsids)
                        journal_labsids.AddRange(db.Journals_Labs.Where(y => y.JournalId == item).Select(x => x.Id));

                    Marks = db.Marks.Include(x => x.Journal_Lab).ThenInclude(x => x.Journal).ThenInclude(x => x.Subject)
                    .Where(x => journal_labsids.Contains(x.Journal_LabId)).ToList();

                    prev_journal_labs = db.Journals_Labs.Include(x => x.Journal).Where(x => x.Journal.GroupId == CurrentGroup.Id)
                        .ToList().Where(x => DateTime.Parse(x.Date) < DateTime.Now.Date).ToList();
                }

                var datatable = new DataTable();

                datatable.Columns.Add(new DataColumn("№"));
                datatable.Columns.Add(new DataColumn("ФИО"));
                datatable.Columns.Add(new DataColumn("Д"));

                foreach (var subject in Subjects)
                    datatable.Columns.Add(new DataColumn() { ColumnName = subject.Id.ToString(), Caption = subject.ShortTitle });

                for (int i = 0; i < Students.Count; i++)
                {
                    var row = datatable.NewRow();

                    row["№"] = i + 1;
                    row["ФИО"] = ShortFullName(Students[i]);

                    uint debts = 0;

                    foreach (var subject in Subjects)
                    {
                        uint countprevs = (uint)prev_journal_labs.Where(x => x.Journal.SubjectId == subject.Id && x.Journal.SubGroup == Students[i].SubGroup).Count();
                        var count = Marks.Where(x => x.StudentId == Students[i].Id && x.Journal_Lab.Journal.SubjectId == subject.Id).Count(x => x.PracticeState != "");
                        
                        row[subject.Id.ToString()] = countprevs - count == 0 ? "" : (countprevs - count).ToString();
                        
                        if (row[subject.Id.ToString()].ToString() != "")
                            debts += uint.Parse(row[subject.Id.ToString()].ToString());
                    }
                    row["Д"] = debts == 0 ? "" : debts.ToString() ; 

                    datatable.Rows.Add(row);
                }
                dataTable = datatable;
            });
        }

        private string ShortFullName(Student student)
        {
            if (student.SecondName != "")
                return $"{student.Surname} {student.FirstName[0]}.{student.SecondName?[0]}.";
            else
                return $"{student.Surname} {student.FirstName[0]}";
        }
    }
}
