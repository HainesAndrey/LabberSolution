using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LabberClient.Workspace.JournalsTab.JournalTable
{
    public partial class JournalTablePage : Page
    {
        public JournalTablePage(Journal journal, ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new JournalTablePageVM(journal, responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }

        //private void SetTrueMark(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    foreach (var row in table.SelectedCells.Select(x => (DataRowView)x.Item).Distinct())
        //    {
        //        var arr = row.Row.ItemArray;
        //        foreach (var index in table.SelectedCells.Where(x => (DataRowView)x.Item == row).Select(x => x.Column.DisplayIndex).Where(x => x > 1))
        //        {
        //            Mark mark = new Mark()
        //            {

        //            }
        //            parametres.Add("fullNameStud", arr[1].ToString());
        //            parametres.Add("idTgs", currentJournal.Id);
        //            parametres.Add("numLab", int.Parse(table.Columns[index].Header.ToString()));
        //            if ((arr[index] is null) || ((string)arr[index] != "зач"))
        //            {
        //                if (!(arr[index] is null) && ((string)arr[index] != ""))
        //                    (DataContext as JournalTablePageVM).SetTrueMarkBody();

        //                arr[index] = "зач";
        //                parametres.Add("state", "зач");
        //                db.CallProcedure("InsertStLab", ref parametres);
        //            }
        //            else
        //            {
        //                arr[index] = "";
        //                db.CallProcedure("DeleteStLab", ref parametres);
        //            }
        //        }


        //        row.Row.ItemArray = arr;
        //    }
        //}

        //private void MoreInfo(object sender, System.Windows.RoutedEventArgs e)
        //{

        //}

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            List<Mark> marks = new List<Mark>();
            foreach (var row in table.SelectedCells.Select(x => (DataRowView)x.Item).Distinct())
            {
                var arr = row.Row.ItemArray;
                foreach (var index in table.SelectedCells.Where(x => (DataRowView)x.Item == row).Select(x => x.Column.DisplayIndex).Where(x => x > 2))
                {
                    marks.Add(new Mark()
                    {
                        Journal_Lab = (DataContext as JournalTablePageVM).Journal_Labs.First(x => x.Lab.Number.ToString() == table.Columns[index].Header.ToString()),
                        Student = (DataContext as JournalTablePageVM).Students.First(x => ShortFullName(x) == arr[1].ToString()),
                        Date = DateTime.Now.ToShortDateString(),
                        PracticeState = arr[index].ToString()
                    });

                    MessageBox.Show(marks.First().PracticeState);

                    //if ((arr[index] is null) || ((string)arr[index] != "зач"))
                    //{
                    //    //if (!(arr[index] is null) && ((string)arr[index] != ""))
                    //    //    db.CallProcedure("DeleteStLab", ref parametres);

                    //    arr[index] = "зач";
                    //    parametres.Add("state", "зач");
                    //    db.CallProcedure("InsertStLab", ref parametres);
                    //}
                    //else
                    //{
                    //    arr[index] = "";
                    //    db.CallProcedure("DeleteStLab", ref parametres);
                    //}
                }


                //row.Row.ItemArray = arr;
            }
            (DataContext as JournalTablePageVM).CurrentItems = marks;
        }

        private Journal_Lab StringToJournal_Lab(string value)
        {
            var items = value.Split('\n');
            return new Journal_Lab() { };
            //return $"{journal_lab.Lab.Number}\n{journal_lab.Date}";
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
