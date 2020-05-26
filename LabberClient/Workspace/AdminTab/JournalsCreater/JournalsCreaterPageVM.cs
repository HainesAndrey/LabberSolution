﻿using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using MvvmCross.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace LabberClient.Workspace.AdminTab.JournalsCreater
{
    public class JournalsCreaterPageVM : LabberVMBase
    {
        private string group;
        private string subGroup;
        private string subject;
        private string teacher;
        private bool addEnabled = false;

        public IEnumerable<string> Groups { get; set; }
        public IEnumerable<string> SubGroups { get; set; }
        public IEnumerable<string> Subjects { get; set; }
        public IEnumerable<string> Teachers { get; set; }

        public ObservableCollection<JournalDTO> Items { get; set; }
        public JournalDTO CurrentItem { get; set; }

        public string Group { get => group;
            set
            {
                group = value;
                AddEnabled = IsAllFileds();
            }
        }
        public string SubGroup { get => subGroup;
            set
            {
                subGroup = value;
                AddEnabled = IsAllFileds();
            }
        }
        public string Subject { get => subject;
            set
            {
                subject = value;
                AddEnabled = IsAllFileds();
            }
        }
        public string Teacher { get => teacher;
            set
            {
                teacher = value;
                AddEnabled = IsAllFileds();
            }
        }

        private bool IsAllFileds()
        {
            return Group != null && SubGroup != null && Subject != null && Teacher != null;
        }

        public bool AddEnabled { get => addEnabled; set { addEnabled = value; RaisePropertyChanged("AddEnabled"); } }

        public MvxCommand Add { get; set; }
        public MvxCommand Delete { get; set; }
        public MvxCommand DeleteAll { get; set; }

        public MvxCommand Complete { get; set; }

        public JournalsCreaterPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            SubGroups = new List<string>() { "1", "2" };
            using (db = new DBWorker())
            {
                Groups = db.Groups.ToList().Select(x => x.Title).OrderBy(x => x);
                Subjects = db.Subjects.ToList().Select(x => x.ShortTitle).OrderBy(x => x);
                Teachers = db.Users.Where(x => x.RoleId == 2).ToList().Select(x => ShortFullName(x)).OrderBy(x => x);
                Items = new ObservableCollection<JournalDTO>(db.Journals.ToList().Select(x => new JournalDTO(x.Id, x.Group.Title, x.SubGroup, x.Subject.ShortTitle, ShortFullName(x.User))));
            }

            var view = (CollectionView)CollectionViewSource.GetDefaultView(Items);
            view.SortDescriptions.Add(new SortDescription("Group", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("SubGroup", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Subject", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Teacher", ListSortDirection.Ascending));

            Add = new MvxCommand(AddBody);
            Delete = new MvxCommand(DeleteBody);
            DeleteAll = new MvxCommand(DeleteAllBody);
        }

        private void DeleteAllBody()
        {
            using (db = new DBWorker())
            {
                db.Journals.RemoveRange(db.Journals);
                db.SaveChanges();
                UpdateItems(db);
            }
        }

        private void DeleteBody()
        {
            using (db = new DBWorker())
            {
                db.Journals.Remove(new Journal() { Id = CurrentItem.Id});
                db.SaveChanges();
                UpdateItems(db);
            }
        }

        private void AddBody()
        {
            using (db = new DBWorker())
            {
                var groupid = db.Groups.First(x => x.Title == Group).Id;
                var subjectid = db.Subjects.First(x => x.ShortTitle == Subject).Id;
                var teacherid = db.Users.Where(x => x.RoleId == 2).ToList().First(x => ShortFullName(x) == Teacher).Id;
                if (!db.Journals.ToList().Exists(x => x.GroupId == groupid && x.SubjectId == subjectid && x.UserId == teacherid && x.SubGroup == SubGroup))
                {
                    db.Journals.Add(new Journal(groupid, subjectid, teacherid, SubGroup));
                    db.SaveChanges();
                    UpdateItems(db);
                }
                else
                    InvokeResponseEvent(ResponseType.Bad, "Такой журнал уже добавлен");
            }
        }

        private void UpdateItems(DBWorker db)
        {
            Items.Clear();
            db.Journals.Include(x => x.Group).Include(x => x.Subject).Include(x => x.User).ToList().ForEach(x => Items.Add(new JournalDTO(x)));
        }

        private string ShortFullName(User user)
        {
            return $"{user.Surname} {user.FirstName[0]}.{user.SecondName[0]}.";
        }
    }
}
