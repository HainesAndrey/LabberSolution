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
        private List<string> groups = new List<string>();
        private List<string> subGroups = new List<string>() { "1", "2" };
        private List<string> subjects = new List<string>();
        private List<string> teachers = new List<string>();
        private List<JournalDTO> items = new List<JournalDTO>();
        private bool deleteAllEnabled;

        public bool DeleteAllEnabled { get => deleteAllEnabled; set { deleteAllEnabled = value; RaisePropertyChanged("DeleteAllEnabled"); } }

        public List<string> Groups { get => groups; set { groups = value; RaisePropertyChanged("Groups"); } }
        public List<string> SubGroups { get => subGroups; set { subGroups = value; RaisePropertyChanged("SubGroups"); } }
        public List<string> Subjects { get => subjects; set { subjects = value; RaisePropertyChanged("Subjects"); } }
        public List<string> Teachers { get => teachers; set { teachers = value; RaisePropertyChanged("Teachers"); } }
        public List<JournalDTO> Items { get => items; set { items = value; RaisePropertyChanged("Items"); } }

        public JournalDTO CurrentItem { get; set; }

        public string Group
        {
            get => group;
            set
            {
                group = value;
                AddEnabled = IsAllFileds();
            }
        }
        public string SubGroup
        {
            get => subGroup;
            set
            {
                subGroup = value;
                AddEnabled = IsAllFileds();
            }
        }
        public string Subject
        {
            get => subject;
            set
            {
                subject = value;
                AddEnabled = IsAllFileds();
            }
        }
        public string Teacher
        {
            get => teacher;
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
            Add = new MvxCommand(AddBody);
            Delete = new MvxCommand(DeleteBody);
            DeleteAll = new MvxCommand(DeleteAllBody);
        }

        public override async void LoadData()
        {
            await Refresh();
        }

        private Task Refresh()
        {
            return Task.Run(() =>
            {
                using (db = new DBWorker())
                {
                    Groups = db.Groups.ToList().Select(x => x.Title).OrderBy(x => x).ToList();
                    Subjects = db.Subjects.ToList().Select(x => x.ShortTitle).OrderBy(x => x).ToList();
                    Teachers = db.Users.Where(x => x.RoleId == 2).ToList().Select(x => ShortFullName(x)).OrderBy(x => x).ToList();
                    Items = db.Journals.Include(x => x.Group).Include(x => x.Subject).Include(x => x.User).ToList().Select(x => new JournalDTO(x)).ToList();
                }
                SubGroups = new List<string>() { "1", "2" };
                DeleteAllEnabled = Items.Count != 0;
            });
        }

        private async void DeleteAllBody()
        {
            using (db = new DBWorker())
            {
                db.Journals.RemoveRange(db.Journals);
            }
            InvokeResponseEvent(ResponseType.Good, "Журналы успешно удалены");
            DeleteAllEnabled = false;
            await Refresh();
        }

        private async void DeleteBody()
        {
            using (db = new DBWorker())
            {
                db.Journals.Remove(new Journal() { Id = CurrentItem.Id });
            }
            InvokeResponseEvent(ResponseType.Good, "Журнал успешно удален");
            await Refresh();
        }

        private async void AddBody()
        {
            using (db = new DBWorker())
            {
                var groupid = db.Groups.First(x => x.Title == Group).Id;
                var subjectid = db.Subjects.First(x => x.ShortTitle == Subject).Id;
                var teacherid = db.Users.Where(x => x.RoleId == 2).ToList().First(x => ShortFullName(x) == Teacher).Id;
                if (db.Journals.ToList().Exists(x => x.GroupId == groupid && x.SubjectId == subjectid && x.UserId == teacherid && x.SubGroup == SubGroup))
                    InvokeResponseEvent(ResponseType.Bad, "Такой журнал уже добавлен");
                else if (db.Journals.ToList().Exists(x => x.GroupId == groupid && x.SubjectId == subjectid && x.SubGroup == SubGroup))
                    InvokeResponseEvent(ResponseType.Bad, "Невозможно добавить журнал, т.к. у такой группы и подгруппы по данной дисциплине уже существует журнал");
                else
                {
                    db.Journals.Add(new Journal(groupid, subjectid, teacherid, SubGroup));
                    InvokeResponseEvent(ResponseType.Good, "Журнал успешно добавлен");
                }   
            }
            await Refresh();
        }

        private string ShortFullName(User user)
        {
            return $"{user.Surname} {user.FirstName[0]}.{user.SecondName[0]}.";
        }
    }
}
