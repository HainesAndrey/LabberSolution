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
using System.Windows.Controls;

namespace LabberClient.Workspace.JournalsTab.JournalTable
{
    public class JournalTablePageVM : LabberVMBase
    {
        private Journal journal;
        public Journal Journal { get => journal; set { journal = value; RaisePropertyChanged("Journal"); } }

        public ObservableCollection<Journal_Lab> Journal_Labs { get; set; }
        public ObservableCollection<Mark> Marks { get; set; }
        public ObservableCollection<Student> Students { get; set; }
        public DataTable DataTable { get; set; }

        public List<Mark> CurrentItems { get; set; }

        //public MvxCommand SetTrueMark { get; set; }

        public JournalTablePageVM(Journal journal, ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            Journal = journal;

            using (db = new DBWorker())
            {
                Journal_Labs = new ObservableCollection<Journal_Lab>(db.Journals_Labs.Include(x => x.Journal).Include(x => x.Lab).Where(x => x.Id == Journal.Id));
                Marks = new ObservableCollection<Mark>(db.Marks.Where(x => x.Journal_Lab.JournalId == Journal.Id));
                Students = new ObservableCollection<Student>(db.Students.Where(x => x.GroupId == Journal.GroupId && x.SubGroup == Journal.SubGroup));
            }

            DataTable = new DataTable();

            //SetTrueMark = new MvxCommand(SetTrueMarkBody);

            DataTable.Columns.Add(new DataColumn("№"));
            DataTable.Columns.Add(new DataColumn("ФИО"));
            DataTable.Columns.Add(new DataColumn("Д"));

            //Journal_Labs = new ObservableCollection<Journal_Lab>() { new Journal_Lab() { Date = $"{DateTime.Now.ToString("dd.MM.yy")}", Lab = new Lab(1, "asdas") } };

            foreach (var journal_lab in Journal_Labs)
                DataTable.Columns.Add(new DataColumn() { ColumnName = journal_lab.Lab.Number.ToString() });


            //Students = new ObservableCollection<Student>() { new Student(0, "asd", "asd", "asdasd", "1") };
            //Marks = new ObservableCollection<Mark>() { new Mark() { Student = Students.First(), Journal_Lab = Journal_Labs.First(), PracticeState = "з." } };

            foreach (var student in Students)
            {
                var row = DataTable.NewRow();

                row["№"] = "1";
                row["ФИО"] = ShortFullName(student);
                row["Д"] = "213";

                foreach (var journal_lab in Journal_Labs)
                    row[journal_lab.Lab.Number.ToString()] = Marks.First().PracticeState;

                DataTable.Rows.Add(row);
            }
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
            return $"{student.Surname} {student.FirstName[0]}.{student.SecondName[0]}.";
        }
    }
}
