using LabberClient.VMStuff;
using LabberClient.Workspace.JournalsTab.JournalsSelector;
using LabberClient.Workspace.JournalsTab.JournalTableWrapper;
using LabberLib.DataBaseContext.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace LabberClient.Workspace.JournalsTab
{
    public class JournalsTabPageVM : LabberVMBase
    {
        public JournalsSelectorPage JournalsSelector { get; set; }

        public ObservableCollection<TabItem> Tabs { get; set; } = new ObservableCollection<TabItem>();
        public List<Journal> Journals { get; set; } = new List<Journal>();

        public JournalsTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            JournalsSelector = new JournalsSelectorPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
            (JournalsSelector.DataContext as JournalsSelectorPageVM).SelectedJournal += JournalsTabPageVM_SelectedJournal;
        }

        private void JournalsTabPageVM_SelectedJournal(Journal journal)
        {
            if (!Journals.Exists(x => x.Id == journal.Id))
            {
                Journals.Add(journal);
                Tabs.Add(new TabItem()
                {
                    Header = GetJournalHeader(journal),
                    Content = new Frame()
                    {
                        Content = new JournalTableWrapperPage(journal, InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent)
                    }
                });
            }
        }

        //public override void LoadData()
        //{
        //    InvokeLoadingStateEvent(true);
        //    Refresh();
        //    InvokeLoadingStateEvent(false);
        //}

        //private async void Refresh()
        //{
        //    //bool isAdmin = false;
        //    //await Task.Run(() =>
        //    //{
        //    //    using (db = new DBWorker())
        //    //    {
        //    //        isAdmin = db.Users.FirstOrDefault(x => x.Id == DBWorker.UserId).RoleId == 1;
        //    //    }
        //    //});
        //}

        private string GetJournalHeader(Journal journal)
        {
            return $"{journal.Group.Title} ({journal.SubGroup}) {journal.Subject.ShortTitle}";
        }
    }
}
