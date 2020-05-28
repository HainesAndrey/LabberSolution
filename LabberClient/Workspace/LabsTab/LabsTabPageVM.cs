using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using MvvmCross.Commands;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LabberClient.Workspace.LabsTab
{
    public class LabsTabPageVM : LabberVMBase
    {
        private bool treeEnabled = true;
        private bool loadingState;
        private ObservableCollection<Node> nodes;
        private bool needToExpandAll;
        private string addSaveBtnTitle = "Добавить";
        private string number;
        private DateTime date = DateTime.Now;
        private List<Journal_Lab> labs;
        private Journal currentJournal;
        private Journal_Lab currentItem;

        public bool TreeEnabled { get => treeEnabled; set { treeEnabled = value; RaisePropertyChanged("TreeEnabled"); } }
        public bool LoadingState { get => loadingState; set { loadingState = value; RaisePropertyChanged("LoadingState"); } }
        public bool NeedToExpandAll { get => needToExpandAll; set { needToExpandAll = value; RaisePropertyChanged("NeedToExpandAll"); } }
        public string AddSaveBtnTitle { get => addSaveBtnTitle; set { addSaveBtnTitle = value; RaisePropertyChanged("AddSaveBtnTitle"); } }
        public string Number { get => number; set { number = value; RaisePropertyChanged("Number"); } }
        public DateTime Date { get => date; set { date = value; RaisePropertyChanged("Date"); } }
        public Journal CurrentJournal { get => currentJournal; set { currentJournal = value; RaisePropertyChanged("CurrentJournal"); } }

        public ObservableCollection<Node> Nodes { get => nodes; set { nodes = value; RaisePropertyChanged("Nodes"); } }
        public List<Journal> Journals { get; set; } = new List<Journal>();
        public List<Journal_Lab> Items { get => labs; set { labs = value; RaisePropertyChanged("Items"); } }
        public Journal_Lab CurrentItem { get => currentItem; set { currentItem = value; RaisePropertyChanged("CurrentItem"); } }

        public MvxCommand GroupByGroups { get; }
        public MvxCommand GroupBySubjects { get; }
        public MvxCommand GroupByTeachers { get; }
        public MvxCommand<bool> ExpandAll { get; }

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
            Refresh();

            GroupByGroups = new MvxCommand(GroupByGroupsBody);
            GroupBySubjects = new MvxCommand(GroupBySubjectsBody);
            GroupByTeachers = new MvxCommand(GroupByTeachersBody);
            ExpandAll = new MvxCommand<bool>(ExpandAllBody);
            Add = new MvxCommand(AddBody);
            Change = new MvxCommand(ChangeBody);
            Delete = new MvxCommand(DeleteBody);
            Clear = new MvxCommand(ClearBody);
            DeleteAll = new MvxCommand(DeleteAllBody);
            AddFromExcel = new MvxCommand(AddFromExcelBody);

            GroupByGroups.Execute();
        }

        public override void LoadData()
        {
            InvokeLoadingStateEvent(true);
            Refresh();
            InvokeLoadingStateEvent(false);
        }

        private async void Refresh()
        {
            bool isAdmin = false;
            await Task.Run(() =>
            {
                using (db = new DBWorker())
                {
                    isAdmin = db.Users.FirstOrDefault(x => x.Id == DBWorker.UserId).RoleId == 1;
                    Journals = db.Journals.Include(x => x.Group).Include(x => x.Subject).Include(x => x.User).ToList();
                    Items = db.Journals_Labs.Include(x => x.Journal).Include(x => x.Lab).ToList().Where(x => x.JournalId == CurrentJournal.Id).ToList();
                }
            });

            GroupByGroups.Execute();
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
                    await Task.Run(() =>
                    {
                        Lab lab = new Lab(double.Parse(Number), "");
                        using (db = new DBWorker())
                        {
                            if (db.Labs.FirstOrDefault(x => x.Number == lab.Number) == null)
                                db.Labs.Add(lab);
                            else
                                lab = db.Labs.ToList().First(x => x.Number == double.Parse(Number));
                            db.SaveChanges();
                            db.Journals_Labs.Include(x => x.Journal).Include(x => x.Lab);
                            db.Journals_Labs.Add(new Journal_Lab() { JournalId = CurrentJournal.Id, Lab = lab, Date = Date.ToString("dd.MM.yyyy") });
                            db.SaveChanges();
                        }
                    });
                    Refresh();
                    InvokeLoadingStateEvent(false);
                }
            }
            else
            {
                //InvokeLoadingStateEvent(true);
                //await Task.Run(() =>
                //{
                //    using (db = new DBWorker())
                //    {
                //        var subject = db.Subjects.FirstOrDefault(x => x.ShortTitle == CurrentItem.ShortTitle && x.LongTitle == CurrentItem.LongTitle);
                //        subject.ShortTitle = ShortTitle;
                //        subject.LongTitle = LongTitle;
                //    }
                //});
                //InvokeLoadingStateEvent(false);
                //Refresh();

                //AddSaveBtnTitle = "Добавить";
                //InvokeResponseEvent(ResponseType.Good, "Диспицлина успешно отредактирована");
                //Clear.Execute();
                //DeleteEnabled = true;
                //DeleteAllEnabled = true;
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




        private void ExpandAllBody(bool state)
        {
            LoadingState = true;
            foreach (var node in Nodes)
                node.AreNodesAxpanded = state;
            LoadingState = false;
        }

        private async void GroupBy(
            IEnumerable<(string title, uint secondId, uint thirdId, string subGroup)> first,
            IEnumerable<(uint id, string title, uint thirdId, string subGroup)> second,
            IEnumerable<(uint id, string title, string subGroup, uint journalId)> third)
        {
            //TreeEnabled = false;
            LoadingState = true;

            List<Node> nodes = null;
            await Task.Run(() =>
            {
                nodes = first.GroupBy(f => f.title).Select(f => new Node()
                {
                    Title = f.Key,
                    Nodes = new ObservableCollection<Node>(second.Where(s => f.ToList().Exists(s1 => s1.secondId == s.id && s1.subGroup == s.subGroup)).GroupBy(s => s.title).Select(s => new Node()
                    {
                        Title = s.Key,
                        Nodes = new ObservableCollection<Node>(third.Where(t => s.ToList().Exists(t1 => t1.thirdId == t.id && t1.subGroup == t.subGroup)).Select(t => new Node()
                        {
                            Title = t.title,
                            IdJournal = t.journalId,
                        }))
                    }))
                }).ToList();
            });
            Nodes = new ObservableCollection<Node>(nodes);
            if (NeedToExpandAll)
                ExpandAll.Execute(true);

            //TreeEnabled = true;
            LoadingState = false;
        }

        private void GroupByTeachersBody()
        {
            GroupBy(Journals.Select(x => (ShortFullName(x.User), x.SubjectId, x.GroupId, x.SubGroup)),
                Journals.Select(x => (x.SubjectId, x.Subject.ShortTitle, x.GroupId, x.SubGroup)),
                Journals.Select(x => (x.GroupId, $"{x.Group.Title} ({x.SubGroup}п/г)", x.SubGroup, x.Id)));
        }

        private void GroupBySubjectsBody()
        {
            GroupBy(Journals.Select(x => (x.Subject.ShortTitle, x.UserId, x.GroupId, x.SubGroup)),
                Journals.Select(x => (x.UserId, ShortFullName(x.User), x.GroupId, x.SubGroup)),
                Journals.Select(x => (x.GroupId, $"{x.Group.Title} ({x.SubGroup}п/г)", x.SubGroup, x.Id)));
        }

        private void GroupByGroupsBody()
        {
            GroupBy(Journals.Select(x => (x.Group.Title, x.SubjectId, x.UserId, x.SubGroup)),
                Journals.Select(x => (x.SubjectId, x.Subject.ShortTitle, x.UserId, x.SubGroup)),
                Journals.Select(x => (x.UserId, ShortFullNameUserAndSubGroup(x.User, x.SubGroup), x.SubGroup, x.Id)));
        }

        public void OpenNewJournal(uint journalId)
        {
            CurrentJournal = Journals.FirstOrDefault(x => x.Id == journalId);
        }

        private string ShortFullNameUserAndSubGroup(User user, string subGroup)
        {
            if (subGroup != "")
                return $"({subGroup}п/г){ShortFullName(user)}";
            else
                return ShortFullName(user);
        }

        private string ShortFullName(User user)
        {
            return $"{user.Surname} {user.FirstName[0]}.{user.SecondName[0]}.";
        }
    }
}
