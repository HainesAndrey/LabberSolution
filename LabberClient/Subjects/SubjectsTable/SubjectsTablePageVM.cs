using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;
using Microsoft.Win32;
using MvvmCross.Commands;
using OfficeOpenXml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

        public string AddSaveBtnTitle { get => addSaveBtnTitle; set { addSaveBtnTitle = value; RaisePropertyChanged("AddSaveBtnTitle"); } }
        public bool AddEnabled { get => addEnabled; set { addEnabled = value; RaisePropertyChanged("AddEnabled"); } }
        public bool DeleteEnabled { get => deleteEnabled; set { deleteEnabled = value; RaisePropertyChanged("DeleteEnabled"); } }
        public bool DeleteAllEnabled { get => deleteAllEnabled; set { deleteAllEnabled = value; RaisePropertyChanged("DeleteAllEnabled"); } }
        public bool ClearEnabled { get => clearEnabled; set { clearEnabled = value; RaisePropertyChanged("ClearEnabled"); } }
        public Subject CurrentItem { get => currentItem; set { currentItem = value; RaisePropertyChanged("CurrentItem"); } }

        public string ShortTitle { get => shortTitle;
            set
            {
                shortTitle = value;
                AddEnabled = CheckRequiredFields();
                ClearEnabled = !IsAllFieldAreEmpty();
                RaisePropertyChanged("ShortTitle");
            }
        }
        public string LongTitle { get => longTitle;
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


        public ObservableCollection<Subject> Items { get; set; }
        public MvxCommand Add { get; set; }
        public MvxCommand Change { get; set; }
        public MvxCommand Delete { get; set; }
        public MvxCommand Clear { get; set; }
        public MvxCommand DeleteAll { get; set; }
        public MvxCommand AddFromExcel { get; set; }

        public SubjectsTablePageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            Items = new ObservableCollection<Subject>();
            var view = (CollectionView)CollectionViewSource.GetDefaultView(Items);
            view.SortDescriptions.Add(new SortDescription("ShortTitle", ListSortDirection.Ascending));

            Add = new MvxCommand(AddBody);
            Change = new MvxCommand(ChangeBody);
            Delete = new MvxCommand(DeleteBody);
            Clear = new MvxCommand(ClearBody);
            DeleteAll = new MvxCommand(DeleteAllBody);
            AddFromExcel = new MvxCommand(AddFromExcelBody);
        }

        public override void LoadData()
        {
            
        }

        private void DeleteAllBody()
        {
            Items.Clear();
        }

        private void ClearBody()
        {
            ShortTitle = "";
            LongTitle = "";
        }

        private void DeleteBody()
        {
            Items.Remove(Items.First(x => x.ShortTitle == ShortTitle));
        }

        private void ChangeBody()
        {
            DeleteAllEnabled = false;
            DeleteEnabled = false;
            ShortTitle = CurrentItem.ShortTitle;
            LongTitle = CurrentItem.LongTitle;
            AddSaveBtnTitle = "Сохранить";
        }

        private void AddBody()
        {
            if (AddSaveBtnTitle == "Добавить")
            {
                if (Items.ToList().Exists(x => x.ShortTitle == ShortTitle))
                    InvokeResponseEvent(ResponseType.Bad, "Дисциплина с такой аббревиатурой уже добавлена");
                else
                    Items.Add(new Subject(ShortTitle, LongTitle));
            }
            else
            {
                var index = Items.IndexOf(CurrentItem);
                Items[index] = new Subject(ShortTitle, LongTitle);
                AddSaveBtnTitle = "Добавить";
                InvokeResponseEvent(ResponseType.Good, "Диспицлина успешно отредактирована");
                Clear.Execute();
                DeleteEnabled = true;
                DeleteAllEnabled = true;
            }
        }

        private void AddFromExcelBody()
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
                object[,] arr;
                try
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
                }
                catch (IOException)
                {
                    InvokeResponseEvent(ResponseType.Bad, "Невозможно открыть файл, т.к. он занят другим процессом");
                    return;
                }

                if (arr is null)
                    InvokeResponseEvent(ResponseType.Bad, "Файл пуст");
                else if (!(arr.GetLength(1) == 2))
                    InvokeResponseEvent(ResponseType.Bad, "Некорректный шаблон файла");
                else
                {
                    for (int i = 0; i < arr.GetLength(0); i++)
                    {
                        var newsubj = new Subject(arr[i, 0].ToString(), arr[i, 1].ToString());
                        if (!Items.ToList().Exists(x => x.ShortTitle == newsubj.ShortTitle))
                            Items.Add(newsubj);
                    }
                    DeleteAllEnabled = true;
                    InvokeResponseEvent(ResponseType.Good, "Дисциплины успешно добавлены из файла");
                }
            }
        }
    }
}
