using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;
using System.Windows.Controls;

namespace LabberClient.Workspace.JournalsTab.JournalTableWrapper
{
    public partial class JournalTableWrapperPage : Page
    {
        public Journal Journal { get; set; }

        public JournalTableWrapperPage(Journal journal, ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
        {
            InitializeComponent();
            Journal = journal;
            DataContext = new JournalTableWrapperPageVM(journal, ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
        }
    }
}
