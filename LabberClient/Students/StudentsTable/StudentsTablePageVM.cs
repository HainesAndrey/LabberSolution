using LabberClient.VMStuff;
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
using System.Windows.Data;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace LabberClient.Students.StudentsTable
{
    public class StudentsTablePageVM : LabberVMBase
    {
        private bool tableEnabled = false;
        private string addSaveBtnTitle = "Добавить";
        private bool addEnabled = false;
        private bool addGroupEnabled = false;
        private bool deleteEnabled = true;
        private bool deleteGroupEnabled = false;
        private bool deleteAllEnabled = false;
        private bool clearEnabled = false;
        private Student currentItem;
        private string surname = "";
        private string firstName = "";
        private string secondName = "";
        private Group currentGroup = new Group("");
        private string groupTitle = "";

        public bool TableEnabled { get => tableEnabled; set { tableEnabled = value; Surname = ""; FirstName = ""; SecondName = ""; RaisePropertyChanged("TableEnabled"); } }
        public string AddSaveBtnTitle { get => addSaveBtnTitle; set { addSaveBtnTitle = value; RaisePropertyChanged("AddSaveBtnTitle"); } }
        public bool AddEnabled { get => addEnabled; set { addEnabled = value; RaisePropertyChanged("AddEnabled"); } }
        public bool AddGroupEnabled { get => addGroupEnabled; set { addGroupEnabled = value; RaisePropertyChanged("AddGroupEnabled"); } }
        public bool DeleteEnabled { get => deleteEnabled; set { deleteEnabled = value; RaisePropertyChanged("DeleteEnabled"); } }
        public bool DeleteGroupEnabled { get => deleteGroupEnabled; set { deleteGroupEnabled = value; RaisePropertyChanged("DeleteGroupEnabled"); } }
        public bool DeleteAllEnabled { get => deleteAllEnabled; set { deleteAllEnabled = value; RaisePropertyChanged("DeleteAllEnabled"); } }
        public bool ClearEnabled { get => clearEnabled; set { clearEnabled = value; RaisePropertyChanged("ClearEnabled"); } }
        public Student CurrentItem { get => currentItem; set { currentItem = value; RaisePropertyChanged("CurrentItem"); } }
        public Group CurrentGroup
        {
            get => currentGroup;
            set
            {
                currentGroup = value;
                DeleteGroupEnabled = Groups.ToList().Exists(x => x.Title == currentGroup?.Title);
                TableEnabled = DeleteGroupEnabled;
                Items.Clear();
                AllStudents.Where(x => x.Group?.Title == currentGroup?.Title).ToList().ForEach(x => Items.Add(x));
                AddGroupEnabled = !Groups.ToList().Exists(x => x.Title == currentGroup?.Title) && currentGroup?.Title != "";
                RaisePropertyChanged("CurrentGroup");
            }
        }
        public string GroupTitle
        {
            get => groupTitle;
            set
            {
                groupTitle = value;
                DeleteGroupEnabled = Groups.ToList().Exists(x => x.Title == groupTitle);
                TableEnabled = DeleteGroupEnabled;
                AddGroupEnabled = !Groups.ToList().Exists(x => x.Title == groupTitle) && groupTitle != "";
                RaisePropertyChanged("GroupTitle");
            }
        }

        public string SecondName { get => secondName; set { secondName = value; RaisePropertyChanged("SecondName"); } }
        public string Surname
        {
            get => surname;
            set
            {
                surname = value;
                AddEnabled = CheckRequiredFields();
                ClearEnabled = !IsAllFieldAreEmpty();
                RaisePropertyChanged("Surname");
            }
        }
        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                AddEnabled = CheckRequiredFields();
                ClearEnabled = !IsAllFieldAreEmpty();
                RaisePropertyChanged("FirstName");
            }
        }

        bool CheckRequiredFields() => Surname != "" && FirstName != "";
        bool IsAllFieldAreEmpty() => Surname == "" && FirstName == "" && SecondName == "";

        public List<Student> AllStudents { get; set; }
        //public ObservableCollection<Group> Groups { get => new ObservableCollection<Group>(db.Groups); set { db.Groups.AddRange(value); db.SaveChanges(); } }
        public ObservableCollection<Student> Items { get; set; }
        public ObservableCollection<Group> Groups { get; set; }
        public MvxCommand Add { get; set; }
        public MvxCommand Change { get; set; }
        public MvxCommand Delete { get; set; }
        public MvxCommand Clear { get; set; }
        public MvxCommand DeleteAll { get; set; }
        public MvxCommand AddFromExcel { get; set; }
        public MvxCommand AddGroup { get; set; }
        public MvxCommand DeleteGroup { get; set; }

        public StudentsTablePageVM(ObservableCollection<Group> groups, List<Student> students, ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            AllStudents = students;
            Groups = groups;
            Items = new ObservableCollection<Student>();
            //db = new DBWorker();

            var view1 = (CollectionView)CollectionViewSource.GetDefaultView(Items);
            view1.SortDescriptions.Add(new SortDescription("SecondName", ListSortDirection.Ascending));
            view1.SortDescriptions.Add(new SortDescription("FirtName", ListSortDirection.Ascending));
            view1.SortDescriptions.Add(new SortDescription("Surname", ListSortDirection.Ascending));

            var view2 = (CollectionView)CollectionViewSource.GetDefaultView(Groups);
            view2.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));

            Add = new MvxCommand(AddBody);
            AddGroup = new MvxCommand(AddGroupBody);
            Change = new MvxCommand(ChangeBody);
            Delete = new MvxCommand(DeleteBody);
            DeleteGroup = new MvxCommand(DeleteGroupBody);
            Clear = new MvxCommand(ClearBody);
            DeleteAll = new MvxCommand(DeleteAllBody);
            AddFromExcel = new MvxCommand(AddFromExcelBody);
        }

        private void DeleteGroupBody()
        {
            Groups.Remove(Groups.First(x => x.Title == GroupTitle));
            AllStudents.RemoveAll(x => x.Group?.Title == GroupTitle);
            GroupTitle = "";
            InvokeResponseEvent(ResponseType.Good, "Группа успешно удалена");
        }

        private void AddGroupBody()
        {
            if (GroupTitle != "" && !Groups.ToList().Exists(x => x.Title == GroupTitle))
            {
                Groups.Add(new Group(GroupTitle));
                CurrentGroup = Groups.First(x => x.Title == GroupTitle);
                InvokeResponseEvent(ResponseType.Good, "Группа успешно добавлена");
            }
        }

        private void DeleteAllBody()
        {
            Items.Clear();
            DeleteAllEnabled = false;
        }

        private void ClearBody()
        {
            Surname = "";
            FirstName = "";
            SecondName = "";
        }

        private void DeleteBody()
        {
            Items.Remove(Items.First(x => x.Surname == Surname && x.FirstName == FirstName && x.SecondName == SecondName));
            if (Items.Count == 0)
                DeleteAllEnabled = false;
        }

        private void ChangeBody()
        {
            DeleteAllEnabled = false;
            DeleteEnabled = false;
            Surname = CurrentItem.Surname;
            FirstName = CurrentItem.FirstName;
            SecondName = CurrentItem.SecondName;
            AddSaveBtnTitle = "Сохранить";
        }

        private void AddBody()
        {
            if (AddSaveBtnTitle == "Добавить")
            {
                if (Items.ToList().Exists(x => x.Surname == Surname && x.FirstName == FirstName && x.SecondName == SecondName))
                    InvokeResponseEvent(ResponseType.Bad, "Такой учащийся уже добавлен");
                else
                {
                    AllStudents.Add(new Student(CurrentGroup.Id, Surname, FirstName, SecondName, "") { Group = CurrentGroup });
                    Items.Add(new Student(CurrentGroup.Id, Surname, FirstName, SecondName, "") { Group = CurrentGroup });
                    DeleteAllEnabled = true;
                    InvokeResponseEvent(ResponseType.Good, "Учащийся успешно добавлен");
                }
            }
            else
            {
                var index = Items.IndexOf(CurrentItem);
                Items[index] = new Student(CurrentGroup.Id, Surname, FirstName, SecondName, "") { Group = CurrentGroup };
                AddSaveBtnTitle = "Добавить";
                InvokeResponseEvent(ResponseType.Good, "Информация об учащемся успешно отредактирована");
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
                else if (!(arr.GetLength(1) == 2 || (arr.GetLength(1) == 3)))
                    InvokeResponseEvent(ResponseType.Bad, "Некорректный шаблон файла");
                else
                {
                    for (int i = 0; i < arr.GetLength(0); i++)
                    {
                        var newstudent = new Student(CurrentGroup.Id, arr[i, 0].ToString(), arr[i, 1].ToString(), arr[i, 2].ToString(), "") { Group = CurrentGroup };
                        if (!Items.ToList().Exists(x => x.Surname == newstudent.Surname && x.FirstName == newstudent.FirstName && x.SecondName == newstudent.SecondName))
                        {
                            Items.Add(newstudent);
                            AllStudents.Add(newstudent);
                        }
                            
                    }
                    //AllStudents.AddRange(Items.Where(x => x.GroupId));
                    //AllStudents = AllStudents.Distinct().ToList();
                    DeleteAllEnabled = true;
                    InvokeResponseEvent(ResponseType.Good, "Дисциплины успешно добавлены из файла");
                }
            }
        }
    }
}
