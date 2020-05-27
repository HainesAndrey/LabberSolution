using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using MvvmCross.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public MvxCommand GroupByGroups { get; }
        public MvxCommand GroupBySubjects { get; }
        public MvxCommand GroupByTeachers { get; }
        public MvxCommand<bool> ExpandAll { get; }

        public bool TreeEnabled { get => treeEnabled; set { treeEnabled = value; RaisePropertyChanged("TreeEnabled"); } }
        public bool LoadingState { get => loadingState; set { loadingState = value; RaisePropertyChanged("LoadingState"); } }
        public bool NeedToExpandAll { get => needToExpandAll; set { needToExpandAll = value; RaisePropertyChanged("NeedToExpandAll"); } }
        public string AddSaveBtnTitle { get => addSaveBtnTitle; set { addSaveBtnTitle = value; RaisePropertyChanged("AddSaveBtnTitle"); } }

        public ObservableCollection<Node> Nodes { get => nodes; set { nodes = value; RaisePropertyChanged("Nodes"); } }
        public List<Journal> Journals { get; set; } = new List<Journal>();

        public LabsTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            Refresh();

            GroupByGroups = new MvxCommand(GroupByGroupsBody);
            GroupBySubjects = new MvxCommand(GroupBySubjectsBody);
            GroupByTeachers = new MvxCommand(GroupByTeachersBody);

            ExpandAll = new MvxCommand<bool>(ExpandAllBody);

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
                }
            });

            GroupByGroups.Execute();
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
            var journal = Journals.FirstOrDefault(x => x.Id == journalId);
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
