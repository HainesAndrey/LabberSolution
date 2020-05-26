using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using MvvmCross.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace LabberClient.Workspace.LabsTab
{
    public class LabsTabPageVM : LabberVMBase
    {
        private bool treeEnabled;
        private bool loadingState;

        public List<Journal> Journals { get; set; } = new List<Journal>();
        public MvxCommand GroupByGroups { get; }
        public MvxCommand GroupBySubjects { get; }
        public MvxCommand GroupByTeachers { get; }
        public MvxCommand<bool> ExpandAll { get; }

        public bool TreeEnabled { get => treeEnabled; set { treeEnabled = value; RaisePropertyChanged("TreeEnabled"); } }
        public bool LoadingState { get => loadingState; set { loadingState = value; RaisePropertyChanged("LoadingState"); } }

        public ObservableCollection<Node> Nodes { get; set; } = new ObservableCollection<Node>();

        public LabsTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            using (db = new DBWorker())
            {
                var isAdmin = db.Users.FirstOrDefault(x => x.Id == DBWorker.UserId).RoleId == 1;
                Journals = db.Journals.Include(x => x.User).Where(x => x.UserId == DBWorker.UserId).Include(x => x.Group).Include(x => x.Subject).ToList();
            }

            GroupByGroups = new MvxCommand(GroupByGroupsBody);
            GroupBySubjects = new MvxCommand(GroupBySubjectsBody);
            GroupByTeachers = new MvxCommand(GroupByTeachersBody);

            ExpandAll = new MvxCommand<bool>(ExpandAllBody);

            GroupByGroups.Execute();
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
