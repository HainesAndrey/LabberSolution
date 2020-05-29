using LabberClient.VMStuff;
using LabberClient.Workspace.JournalsTab.JournalsSelector;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using MvvmCross.Commands;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LabberClient.Workspace.LabsTab
{
    public class LabsTabPageVM : LabberVMBase
    {
        private string addSaveBtnTitle = "Добавить";
        private string number;
        private DateTime date = DateTime.Now;
        private List<Journal_Lab> labs;
        private Journal currentJournal;
        private Journal_Lab currentItem;
        private bool tableEnabled = false;

        public string AddSaveBtnTitle { get => addSaveBtnTitle; set { addSaveBtnTitle = value; RaisePropertyChanged("AddSaveBtnTitle"); } }
        public string Number { get => number; set { number = value; RaisePropertyChanged("Number"); } }
        public DateTime Date { get => date;
            set { date = value; RaisePropertyChanged("Date"); } }
        public bool TableEnabled { get => tableEnabled; set { tableEnabled = value; RaisePropertyChanged("TableEnabled"); } }

        public Journal CurrentJournal { get => currentJournal; set { currentJournal = value; RaisePropertyChanged("CurrentJournal"); } }
        public List<Journal_Lab> Items { get => labs; set { labs = value; RaisePropertyChanged("Items"); } }
        public Journal_Lab CurrentItem { get => currentItem; set { currentItem = value; RaisePropertyChanged("CurrentItem"); } }

        public JournalsSelectorPage JournalsSelectorPage { get; set; }

        public MvxCommand Add { get; }
        public MvxCommand Change { get; }
        public MvxCommand Delete { get; }
        public MvxCommand Clear { get; }
        public MvxCommand DeleteAll { get; }
        public MvxCommand AddFromExcel { get; }
        public bool DeleteAllEnabled { get; private set; }
        public bool DeleteEnabled { get; private set; }

        public LabsTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            Add = new MvxCommand(AddBody);
            Change = new MvxCommand(ChangeBody);
            Delete = new MvxCommand(DeleteBody);
            Clear = new MvxCommand(ClearBody);
            DeleteAll = new MvxCommand(DeleteAllBody);
            AddFromExcel = new MvxCommand(AddFromExcelBody);

            JournalsSelectorPage = new JournalsSelectorPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
            (JournalsSelectorPage.DataContext as JournalsSelectorPageVM).SelectedJournal += LabsTabPageVM_SelectedJournal;
        }

        private void LabsTabPageVM_SelectedJournal(Journal journal)
        {
            CurrentJournal = journal;
            TableEnabled = true;
            Refresh();
        }

        public override void LoadData()
        {
            InvokeLoadingStateEvent(true);
            Refresh();
            InvokeLoadingStateEvent(false);
        }

        private async void Refresh()
        {
            await Task.Run(() =>
            {
                using (db = new DBWorker())
                {
                    //Journals = db.Journals.Include(x => x.Group).Include(x => x.Subject).Include(x => x.User).ToList();
                    if (CurrentJournal != null)
                        Items = db.Journals_Labs.Include(x => x.Journal)
                            .Include(x => x.Lab).ToList().Where(x => x.JournalId == CurrentJournal.Id).ToList();
                }
            });

            //GroupByGroups.Execute();
        }

        private void DeleteAllBody()
        {
            using (db = new DBWorker())
            {
                db.Journals_Labs.RemoveRange(Items);
            }
            Refresh();
        }

        private void ClearBody()
        {
            Number = "";
            Date = DateTime.Now;
        }

        private void DeleteBody()
        {
            using (db = new DBWorker())
            {
                db.Labs.Remove(CurrentItem.Lab);
            }
            Refresh();
        }

        private void ChangeBody()
        {
            DeleteAllEnabled = false;
            DeleteEnabled = false;
            Number = CurrentItem.Lab.Number.ToString();
            Date = DateTime.Parse(CurrentItem.Date);
            AddSaveBtnTitle = "Сохранить";
        }

        private async void AddBody()
        {
            if (AddSaveBtnTitle == "Добавить")
            {
                if (Items.ToList().Exists(x => x.Lab.Number == double.Parse(Number)))
                    InvokeResponseEvent(ResponseType.Bad, "Лабораторная работа с таким номером уже добавлена");
                else
                {
                    InvokeLoadingStateEvent(true);
                    Lab lab = new Lab(double.Parse(Number), "");
                    await Task.Run(() =>
                    {
                        using (db = new DBWorker())
                        {
                            if (db.Labs.ToList().FirstOrDefault(x => x.Number == lab.Number) == null)
                                db.Labs.Add(lab);
                            else
                                lab = db.Labs.ToList().First(x => x.Number == lab.Number);
                            db.SaveChanges();
                            db.Journals_Labs.Include(x => x.Journal).Include(x => x.Lab);
                            db.Journals_Labs.Add(new Journal_Lab() { JournalId = CurrentJournal.Id, Lab = lab, Date = Date.ToString("dd.MM.yy") });
                            db.SaveChanges();
                        }
                    });
                    InvokeResponseEvent(ResponseType.Good, "Лабораторная работа добавлена");
                    Refresh();
                    InvokeLoadingStateEvent(false);
                }
            }
            else
            {
                InvokeLoadingStateEvent(true);
                await Task.Run(() =>
                {
                    using (db = new DBWorker())
                    {
                        var journal_lab = db.Journals_Labs.Include(x => x.Lab).FirstOrDefault(x => x.Id == CurrentItem.Id);
                        journal_lab.Lab.Number = double.Parse(Number);
                        journal_lab.Date = Date.ToString("dd.MM.yy");
                    }
                });
                InvokeLoadingStateEvent(false);
                Refresh();

                AddSaveBtnTitle = "Добавить";
                InvokeResponseEvent(ResponseType.Good, "Лабораторная работа успешно отредактирована");
                Clear.Execute();
                DeleteEnabled = true;
                DeleteAllEnabled = true;
            }
        }

        private async void AddFromExcelBody()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = "shell:MyComputerFolder",
                DefaultExt = ".xlsx",
                Title = "Выберите файл Excel",
                Filter = "Файл Excel|*.xlsx"
            };
            if ((bool)openFileDialog.ShowDialog())
            {
                InvokeLoadingStateEvent(true);
                InvokePageEnabledEvent(false);
                object[,] arr = null;
                try
                {
                    await Task.Run(() =>
                    {
                        using (var fs = new FileStream(openFileDialog.FileName, FileMode.Open))
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            ExcelPackage excel = new ExcelPackage(fs);
                            excel.Load(fs);

                            try
                            {
                                arr = (object[,])excel.Workbook.Worksheets[0].Cells.Value;
                            }
                            catch (InvalidOperationException)
                            {
                                InvokeResponseEvent(ResponseType.Bad, "Невозможно открыть файл, т.к. он поврежден");
                                return;
                            }
                            excel.Dispose();
                        }
                    });
                }
                catch (IOException)
                {
                    InvokeResponseEvent(ResponseType.Bad, "Невозможно открыть файл, т.к. он занят другим процессом");
                    InvokeLoadingStateEvent(false);
                    InvokePageEnabledEvent(true);
                    return;
                }

                if (arr is null)
                    InvokeResponseEvent(ResponseType.Bad, "Файл пуст");
                else if (!(arr.GetLength(1) == 2))
                    InvokeResponseEvent(ResponseType.Bad, "Некорректный шаблон файла");
                else
                {
                    List<Subject> subjects = new List<Subject>();
                    await Task.Run(() =>
                    {
                        for (int i = 0; i < arr.GetLength(0); i++)
                        {
                            var newsubj = new Subject(arr[i, 0].ToString(), arr[i, 1].ToString());
                            if (!subjects.ToList().Exists(x => x.ShortTitle == newsubj.ShortTitle))
                                subjects.Add(newsubj);
                        }
                        using (db = new DBWorker())
                        {
                            db.Subjects.AddRange(subjects);
                        }
                    });
                    Refresh();
                    DeleteAllEnabled = true;
                    InvokeResponseEvent(ResponseType.Good, "Дисциплины успешно добавлены из файла");
                    InvokeLoadingStateEvent(false);
                    InvokePageEnabledEvent(true);
                }
            }
        }
    }
}
