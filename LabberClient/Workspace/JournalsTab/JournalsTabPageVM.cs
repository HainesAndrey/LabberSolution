using LabberClient.VMStuff;
using LabberClient.Workspace.JournalsTab.JournalTableWrapper;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using MvvmCross.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LabberClient.Workspace.JournalsTab
{
    public class JournalsTabPageVM : LabberVMBase
    {
        private bool treeEnabled = true;
        private bool loadingState;
        private Visibility filterEnabled;

        public bool TreeEnabled { get => treeEnabled; set { treeEnabled = value; RaisePropertyChanged("TreeEnabled"); } }
        public bool LoadingState { get => loadingState; set { loadingState = value; RaisePropertyChanged("LoadingState"); } }
        public Visibility FilterEnabled { get => filterEnabled; set { filterEnabled = value; RaisePropertyChanged("FilterEnabled"); } }

        public MvxCommand GroupByGroups { get; set; }
        public MvxCommand GroupBySubjects { get; set; }
        public MvxCommand GroupByTeachers { get; set; }
        public MvxCommand FilterByOwn { get; set; }
        public MvxCommand FilterByAll { get; set; }
        public MvxCommand<bool> ExpandAll { get; set; }

        public ObservableCollection<Node> Nodes { get; set; } = new ObservableCollection<Node>();
        public ObservableCollection<TabItem> Tabs { get; set; } = new ObservableCollection<TabItem>();
        public List<Journal> Journals { get; set; } = new List<Journal>();

        public JournalsTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            //Journals = new List<Journal>()
            //{
            //    new Journal(1, 1, 1, "1")
            //    {
            //        Id = 1,
            //        Group = new Group("П-1722") { Id = 1 },
            //        Subject = new Subject("БДиСУБД", "Базы данных и сисетмы управления базами данных") { Id = 1 },
            //        User = new User() { Surname = "Доманова", FirstName = "Юлия", SecondName = "Анатольенва", Id = 1 }
            //    },
            //    new Journal(2, 2, 2, "1")
            //    {
            //        Id = 2,
            //        Group = new Group("П-42") { Id = 2 },
            //        Subject = new Subject("ПООГИ", "Программное обеспечение информационных технологий") { Id = 2 },
            //        User = new User() { Surname = "Меньшикова", FirstName = "Марина", SecondName = "Валерьвена", Id = 2 }
            //    },
            //    new Journal(1, 3, 3, "2")
            //    {
            //        Id = 3,
            //        Group = new Group("П-1722") { Id = 1 },
            //        Subject = new Subject("СиАОД", "Структуры и алогритмы обработки данных") { Id = 3 },
            //        User = new User() { Surname = "Дроздова", FirstName = "Юлия", SecondName = "Анатольенва", Id = 3 }
            //    },
            //};

            using (db = new DBWorker())
            {
                var isAdmin = db.Users.FirstOrDefault(x => x.Id == DBWorker.UserId).RoleId == 1;
                FilterEnabled = isAdmin ? Visibility.Collapsed : Visibility.Visible;
                Journals = db.Journals.Include(x => x.Group).Include(x => x.Subject).Include(x => x.User).ToList();
            }


            GroupByGroups = new MvxCommand(GroupByGroupsBody);
            GroupBySubjects = new MvxCommand(GroupBySubjectsBody);
            GroupByTeachers = new MvxCommand(GroupByTeachersBody);

            FilterByOwn = new MvxCommand(FilterByOwnBody);
            FilterByAll = new MvxCommand(FilterByAllBody);

            ExpandAll = new MvxCommand<bool>(ExpandAllBody);

            GroupByGroups.Execute();
            FilterByOwn.Execute();
        }

        public override void LoadData()
        {
            
        }

        public void OpenNewJournal(uint journalId)
        {
            var journal = Journals.FirstOrDefault(x => x.Id == journalId);
            if (journal != null)
                Tabs.Add(new TabItem()
                {
                    Header = GetJournalHeader(journal),
                    Content = new Frame()
                    {
                        Content = new JournalTableWrapperPage(journal, InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent)
                    }
                });
        }

        private string GetJournalHeader(Journal journal)
        {
            return $"{journal.Group.Title} ({journal.SubGroup}) {journal.Subject.ShortTitle}";
        }

        private void ExpandAllBody(bool state)
        {
            LoadingState = true;
            foreach (var node in Nodes)
                node.AreNodesAxpanded = state;
            LoadingState = false;
        }

        private void GroupBy(
            IEnumerable<(string title, uint secondId, uint thirdId, string subGroup)> first,
            IEnumerable<(uint id, string title, uint thirdId, string subGroup)> second,
            IEnumerable<(uint id, string title, string subGroup, uint journalId)> third)
        {
            TreeEnabled = false;
            LoadingState = true;
            Nodes.Clear();

            first.GroupBy(f => f.title).Select(f => new Node()
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
            }).ToList().ForEach(x => Nodes.Add(x));

            TreeEnabled = true;
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

        private void FilterByAllBody()
        {

        }

        private void FilterByOwnBody()
        {

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
