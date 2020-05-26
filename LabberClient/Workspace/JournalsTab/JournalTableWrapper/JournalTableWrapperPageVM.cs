using LabberClient.VMStuff;
using LabberClient.Workspace.JournalsTab.JournalTable;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using System.Collections.ObjectModel;
using System.Linq;

namespace LabberClient.Workspace.JournalsTab.JournalTableWrapper
{
    public class JournalTableWrapperPageVM : LabberVMBase
    {
        private Journal journal;
        private string title;

        public Journal Journal { get => journal; set { journal = value; RaisePropertyChanged("Journal"); } }
        public string Title { get => title; set { title = value; RaisePropertyChanged("Title"); } }

        public JournalTablePage JournalTablePage { get; set; }

        public JournalTableWrapperPageVM(Journal journal, ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            Journal = journal;

            JournalTablePage = new JournalTablePage(journal, InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);

            Title = $"\"{Journal.Subject.LongTitle}\"\n{FullName(Journal.User)}\n{Journal.Group.Title} {Journal.SubGroup} п/г";
        }

        private string FullName(User user)
        {
            return $"{user.Surname} {user.FirstName} {user.SecondName}";
        }
    }
}
