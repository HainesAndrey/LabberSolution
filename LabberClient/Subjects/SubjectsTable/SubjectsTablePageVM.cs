using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using Microsoft.Win32;
using MvvmCross.Commands;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace LabberClient.Subjects.SubjectsTable
{
    public class SubjectsTablePageVM : LabberVMBase
    {
        private string addSaveBtnTitle = "Добавить";
        private bool addEnabled = false;
        private bool deleteEnabled = true;
        private bool deleteAllEnabled = false;
        private bool clearEnabled = false;
        private Subject currentItem;
        private string shortTitle = "";
        private string longTitle = "";
        private List<Subject> items = new List<Subject>();

        public string AddSaveBtnTitle { get => addSaveBtnTitle; set { addSaveBtnTitle = value; RaisePropertyChanged("AddSaveBtnTitle"); } }
        public bool AddEnabled { get => addEnabled; set { addEnabled = value; RaisePropertyChanged("AddEnabled"); } }
        public bool DeleteEnabled { get => deleteEnabled; set { deleteEnabled = value; RaisePropertyChanged("DeleteEnabled"); } }
        public bool DeleteAllEnabled { get => deleteAllEnabled; set { deleteAllEnabled = value; RaisePropertyChanged("DeleteAllEnabled"); } }
        public bool ClearEnabled { get => clearEnabled; set { clearEnabled = value; RaisePropertyChanged("ClearEnabled"); } }
        public Subject CurrentItem { get => currentItem; set { currentItem = value; RaisePropertyChanged("CurrentItem"); } }

        public string ShortTitle
        {
            get => shortTitle;
            set
            {
                shortTitle = value;
                AddEnabled = CheckRequiredFields();
                ClearEnabled = !IsAllFieldAreEmpty();
                RaisePropertyChanged("ShortTitle");
            }
        }
        public string LongTitle
        {
            get => longTitle;
            set
            {
                longTitle = value;
                AddEnabled = CheckRequiredFields();
                ClearEnabled = !IsAllFieldAreEmpty();
                RaisePropertyChanged("LongTitle");
            }
        }

        bool CheckRequiredFields() => ShortTitle != "" && LongTitle != "";
        bool IsAllFieldAreEmpty() => ShortTitle == "" && LongTitle == "";

        public List<Subject> Items { get => items; set { items = value; RaisePropertyChanged("Items"); } }
        public MvxCommand Add { get; set; }
        public MvxCommand Change { get; set; }
        public MvxCommand Delete { get; set; }
        public MvxCommand Clear { get; set; }
        public MvxCommand DeleteAll { get; set; }
        public MvxCommand AddFromExcel { get; set; }

        public SubjectsTablePageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            Add = new MvxCommand(AddBody);
            Change = new MvxCommand(ChangeBody);
            Delete = new MvxCommand(DeleteBody);
            Clear = new MvxCommand(ClearBody);
            DeleteAll = new MvxCommand(DeleteAllBody);
            AddFromExcel = new MvxCommand(AddFromExcelBody);
        }

        public override void LoadData()
        {
            Refresh();
        }

        private async void Refresh()
        {
            InvokeLoadingStateEvent(true);
            await Task.Run(() =>
            {
                using (db = new DBWorker())
                {
                    Items = db.Subjects.ToList().OrderBy(x => x.ShortTitle).ToList();
                }
            });
            DeleteAllEnabled = Items.Count > 0;
            InvokeLoadingStateEvent(false);
        }

        private void DeleteAllBody()
        {
            using (db = new DBWorker())
            {
                db.Subjects.RemoveRange(Items);
            }
            InvokeResponseEvent(ResponseType.Good, "Дисциплины успешно удалены");
            Refresh();
        }

        private void ClearBody()
        {
            ShortTitle = "";
            LongTitle = "";
        }

        private void DeleteBody()
        {
            using (db = new DBWorker())
            {
                db.Subjects.Remove(db.Subjects.First(x => x.ShortTitle == CurrentItem.ShortTitle));
            }
            InvokeResponseEvent(ResponseType.Good, "Дисциплина успешно удалена");
            Refresh();
        }

        private void ChangeBody()
        {
            DeleteAllEnabled = false;
            DeleteEnabled = false;
            ShortTitle = CurrentItem.ShortTitle;
            LongTitle = CurrentItem.LongTitle;
            AddSaveBtnTitle = "Сохранить";
        }

        private async void AddBody()
        {
            if (AddSaveBtnTitle == "Добавить")
            {
                if (ShortTitle.Replace(" ", "") == "" || LongTitle.Replace(" ", "") == "")
                    InvokeResponseEvent(ResponseType.Bad, "Информация о дисциплине некорректна");
                else if (Items.ToList().Exists(x => x.ShortTitle == ShortTitle))
                    InvokeResponseEvent(ResponseType.Bad, "Дисциплина с такой аббревиатурой уже добавлена");
                else
                {
                    InvokeLoadingStateEvent(true);
                    await Task.Run(() =>
                    {
                        using (db = new DBWorker())
                        {
                            db.Subjects.Add(new Subject(ShortTitle, LongTitle));
                        }
                    });
                    InvokeResponseEvent(ResponseType.Good, "Дисциплина успешно добавлена");
                    Refresh();
                    InvokeLoadingStateEvent(false);
                }
            }
            else
            {
                if (ShortTitle.Replace(" ", "") == "" || LongTitle.Replace(" ", "") == "")
                {
                    InvokeResponseEvent(ResponseType.Bad, "Информация о дисциплине некорректна");
                    return;
                }
                InvokeLoadingStateEvent(true);
                await Task.Run(() =>
                {
                    using (db = new DBWorker())
                    {
                        var subject = db.Subjects.FirstOrDefault(x => x.ShortTitle == CurrentItem.ShortTitle && x.LongTitle == CurrentItem.LongTitle);
                        subject.ShortTitle = ShortTitle;
                        subject.LongTitle = LongTitle;
                    }
                });
                InvokeLoadingStateEvent(false);
                Refresh();

                AddSaveBtnTitle = "Добавить";
                InvokeResponseEvent(ResponseType.Good, "Диспицлина успешно отредактирована");
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
                            {
                                subjects.Add(newsubj);
                                using (db = new DBWorker())
                                {
                                    db.Subjects.Add(newsubj);
                                }
                            }
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
