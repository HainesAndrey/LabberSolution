using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using MvvmCross.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LabberClient.Workspace.JournalsTab.JournalTable
{
    public class JournalTablePageVM : LabberVMBase
    {
        private Journal journal;
        private DataTable dataTable;

        public Journal Journal { get => journal; set { journal = value; RaisePropertyChanged("Journal"); } }

        public ObservableCollection<Journal_Lab> Journal_Labs { get; set; }
        public ObservableCollection<Mark> Marks { get; set; }
        public ObservableCollection<Student> Students { get; set; }
        public DataTable DataTable { get => dataTable; set { dataTable = value; RaisePropertyChanged("DataTable"); } }

        public List<Mark> CurrentItems { get; set; }

        public MvxCommand SetTrueMark { get; set; }

        public JournalTablePageVM(Journal journal, ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            Journal = journal;
            SetTrueMark = new MvxCommand(SetTrueMarkBody);

            //Refresh().Wait();
        }

        private void SetTrueMarkBody()
        {
            throw new NotImplementedException();
        }

        public async override void LoadData()
        {
            await Refresh();
        }

        private Task Refresh()
        {
            return Task.Run(() =>
            {
                using (db = new DBWorker())
                {
                    Journal_Labs = new ObservableCollection<Journal_Lab>(db.Journals_Labs.Include(x => x.Journal).Include(x => x.Lab).Where(x => x.JournalId == Journal.Id));
                    Marks = new ObservableCollection<Mark>(db.Marks.Where(x => x.Journal_Lab.JournalId == Journal.Id));
                    Students = new ObservableCollection<Student>(db.Students.Where(x => x.GroupId == Journal.GroupId && x.SubGroup == Journal.SubGroup)
                        .OrderBy(x => x.Surname).ThenBy(x => x.FirstName).ThenBy(x => x.SecondName));
                }

                DataTable = new DataTable();

                DataTable.Columns.Add(new DataColumn("№"));
                DataTable.Columns.Add(new DataColumn("ФИО"));
                DataTable.Columns.Add(new DataColumn("Д"));

                foreach (var journal_lab in Journal_Labs)
                    DataTable.Columns.Add(new DataColumn() { ColumnName = journal_lab.Lab.Number.ToString() });

                for (int i = 0; i < Students.Count; i++)
                {
                    var row = DataTable.NewRow();

                    row["№"] = i + 1;
                    row["ФИО"] = ShortFullName(Students[i]);
                    row["Д"] = "";

                    foreach (var journal_lab in Journal_Labs)
                        row[journal_lab.Lab.Number.ToString()] = Marks.FirstOrDefault()?.PracticeState;

                    DataTable.Rows.Add(row);
                }
            });
        }

        public void SetTrueMarkBody(Mark mark)
        {
            using (db = new DBWorker())
            {
                if (mark.PracticeState != "")
                    db.Marks.Add(mark);
                else
                    db.Marks.Remove(mark);
                db.SaveChanges();
            }
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
