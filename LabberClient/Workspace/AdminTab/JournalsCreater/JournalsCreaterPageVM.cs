using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using MvvmCross.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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

        public ObservableCollection<string> Groups { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> SubGroups { get; set; } = new ObservableCollection<string>() { "1", "2" };
        public ObservableCollection<string> Subjects { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Teachers { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<JournalDTO> Items { get; set; } = new ObservableCollection<JournalDTO>();
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

        public JournalsCreaterPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            var view = (CollectionView)CollectionViewSource.GetDefaultView(Items);
            view.SortDescriptions.Add(new SortDescription("Group", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("SubGroup", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Subject", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Teacher", ListSortDirection.Ascending));

            Add = new MvxCommand(AddBody);
            Delete = new MvxCommand(DeleteBody);
            DeleteAll = new MvxCommand(DeleteAllBody);
        }

        public override void LoadData()
        {
            Refresh();
        }

        private async void Refresh()
        {
            Items.Clear();
            Groups.Clear();
            Subjects.Clear();
            Teachers.Clear();

            List<string> groups = null;
            List<string> subjects = null;
            List<string> users = null;
            List<Journal> journals = null;

            await Task.Run(() =>
            {
                using (db = new DBWorker())
                {
                    groups = db.Groups.ToList().Select(x => x.Title).OrderBy(x => x).ToList();
                    subjects = db.Subjects.ToList().Select(x => x.ShortTitle).OrderBy(x => x).ToList();
                    users = db.Users.Where(x => x.RoleId == 2).ToList().Select(x => ShortFullName(x)).OrderBy(x => x).ToList();
                    journals = db.Journals.Include(x => x.Group).Include(x => x.Subject).Include(x => x.User).ToList();
                }
            });

            groups?.ForEach(x => Groups.Add(x));
            subjects?.ForEach(x => Subjects.Add(x));
            users?.ForEach(x => Teachers.Add(x));
            journals?.ForEach(x => Items.Add(new JournalDTO(x)));
        }

        private void DeleteAllBody()
        {
            using (db = new DBWorker())
            {
                db.Journals.RemoveRange(db.Journals);
            }
            Refresh();
        }

        private void DeleteBody()
        {
            using (db = new DBWorker())
            {
                db.Journals.Remove(new Journal() { Id = CurrentItem.Id});
            }
            Refresh();
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
                }
                else
                    InvokeResponseEvent(ResponseType.Bad, "Такой журнал уже добавлен");
            }
            Refresh();
        }

        private string ShortFullName(User user)
        {
            return $"{user.Surname} {user.FirstName[0]}.{user.SecondName[0]}.";
        }
    }
}
