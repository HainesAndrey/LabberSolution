using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LabberClient.Workspace.JournalsTab.JournalTable
{
    public class JournalTablePageVM : LabberVMBase
    {
        private Journal journal;
        private DataTable dataTable;
        private Visibility trueStateClickState;

        public Journal Journal { get => journal; set { journal = value; RaisePropertyChanged("Journal"); } }

        public ObservableCollection<Journal_Lab> Journal_Labs { get; set; }
        public ObservableCollection<Mark> Marks { get; set; }
        public ObservableCollection<Student> Students { get; set; }
        //public DataTable DataTable { get => dataTable; set { dataTable = value; RaisePropertyChanged("DataTable"); } }
        public bool CanEdit { get; set; }
        public Visibility TrueStateClickState { get => trueStateClickState; set { trueStateClickState = value; RaisePropertyChanged("TrueStateClickState"); } }

        public Mark CurrentMark { get; set; }

        public delegate void UpdateHeadersEventHandler(DataTable table);
        public event UpdateHeadersEventHandler UpdateHeaders;

        public JournalTablePageVM(Journal journal, ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            Journal = journal;
            CanEdit = DBWorker.UserId == Journal.UserId;
            TrueStateClickState = CanEdit ? Visibility.Visible : Visibility.Hidden;
        }

        public async override void LoadData()
        {
            await Refresh();
            UpdateHeaders?.Invoke(dataTable);
        }

        private Task Refresh()
        {
            return Task.Run(() =>
            {
                using (db = new DBWorker())
                {
                    Journal_Labs = new ObservableCollection<Journal_Lab>(db.Journals_Labs.Include(x => x.Journal).Include(x => x.Lab).Where(x => x.JournalId == Journal.Id));
                    Marks = new ObservableCollection<Mark>(db.Marks.Include(x => x.Journal_Lab).Where(x => x.Journal_Lab.JournalId == Journal.Id));
                    Students = new ObservableCollection<Student>(db.Students.Where(x => x.GroupId == Journal.GroupId && x.SubGroup == Journal.SubGroup)
                        .OrderBy(x => x.Surname).ThenBy(x => x.FirstName).ThenBy(x => x.SecondName));
                }

                var datatable = new DataTable();

                datatable.Columns.Add(new DataColumn("№"));
                datatable.Columns.Add(new DataColumn("ФИО"));
                datatable.Columns.Add(new DataColumn("Д"));

                foreach (var journal_lab in Journal_Labs)
                    datatable.Columns.Add(new DataColumn() { ColumnName = journal_lab.Id.ToString(), Caption = Journal_LabToString(journal_lab) });

                for (int i = 0; i < Students.Count; i++)
                {
                    var row = datatable.NewRow();

                    row["№"] = i + 1;
                    row["ФИО"] = ShortFullName(Students[i]);
                    row["Д"] = Marks.Where(x => x.StudentId == Students[i].Id).Count(x => DateTime.Parse(x.Journal_Lab.Date) < DateTime.Now.Date);

                    foreach (var journal_lab in Journal_Labs)
                        row[journal_lab.Id.ToString()] = Marks.FirstOrDefault(x => x.StudentId == Students[i].Id && x.Journal_LabId == journal_lab.Id)?.PracticeState == "з." ? "зач" : "";

                    datatable.Rows.Add(row);
                }
                dataTable = datatable;
            });
        }

        public async void SetTrueMark()
        {
            using (db = new DBWorker())
            {
                if (CurrentMark.Id == 0)
                {
                    CurrentMark.PracticeState = "з.";
                    db.Marks.Add(CurrentMark);
                }
                else
                {
                    var mark = db.Marks.FirstOrDefault(x => x.Id == CurrentMark.Id);
                    if (CurrentMark.PracticeState != "з.")
                        mark.PracticeState = "з.";
                    else
                        mark.PracticeState = "";
                }
            }
            await Refresh();
            UpdateHeaders?.Invoke(dataTable);
        }

        private string Journal_LabToString(Journal_Lab journal_lab)
        {
            return $"{journal_lab.Lab.Number}\n{journal_lab.Date}";
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
