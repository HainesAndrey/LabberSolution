using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace LabberClient.Workspace.JournalsTab.JournalTable
{
    public partial class JournalTablePage : Page
    {
        public JournalTablePage(Journal journal, ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new JournalTablePageVM(journal, responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
            (DataContext as JournalTablePageVM).UpdateHeaders += JournalTablePage_UpdateHeaders;
        }

        private void JournalTablePage_UpdateHeaders(DataTable datatable)
        {
            table.ItemsSource = datatable.DefaultView;
            foreach (var item in table.Columns)
            {
                item.Header = datatable.Columns[item.Header.ToString()].Caption;
            }
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (!(DataContext as JournalTablePageVM).CanEdit)
                return;
            List<Mark> marks = new List<Mark>();
            foreach (var row in table.SelectedCells.Select(x => (DataRowView)x.Item).Distinct())
            {
                var arr = row.Row.ItemArray;
                foreach (var index in table.SelectedCells.Where(x => (DataRowView)x.Item == row).Select(x => x.Column.DisplayIndex).Where(x => x > 2))
                {
                    marks.Add((DataContext as JournalTablePageVM).Marks.FirstOrDefault(x => x.Journal_Lab == (DataContext as JournalTablePageVM).Journal_Labs.First(x => Journal_LabToString(x) == table.Columns[index].Header.ToString())
                    && x.Student == (DataContext as JournalTablePageVM).Students.First(x => ShortFullName(x) == arr[1].ToString())));
                    if (marks?.FirstOrDefault() == null)
                    {
                        marks.Clear();
                        var mark = new Mark()
                        {
                            Journal_LabId = (DataContext as JournalTablePageVM).Journal_Labs.FirstOrDefault(x => Journal_LabToString(x) == table.Columns[index].Header.ToString())?.Id ?? 0,
                            StudentId = (DataContext as JournalTablePageVM).Students.FirstOrDefault(x => ShortFullName(x) == arr[1].ToString())?.Id ?? 0,
                        };
                        mark.Date = (DataContext as JournalTablePageVM).Journal_Labs.FirstOrDefault(x => x.Id == mark.Journal_LabId).Date;
                        marks.Add(mark);
                    }
                        
                }
            }
            (DataContext as JournalTablePageVM).CurrentMark = marks?.FirstOrDefault();
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as LabberVMBase).LoadData();
        }

        private void TrueState_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as JournalTablePageVM).SetTrueMark();
        }

        //private void MarkState_Click(object sender, RoutedEventArgs e)
        //{

        //}
    }
}
