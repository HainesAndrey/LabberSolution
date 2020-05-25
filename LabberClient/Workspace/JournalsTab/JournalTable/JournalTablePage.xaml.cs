using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;
using System.Windows.Controls;

namespace LabberClient.Workspace.JournalsTab.JournalTable
{
    public partial class JournalTablePage : Page
    {
        public JournalTablePage(Journal journal, ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
        {
            InitializeComponent();
            DataContext = new JournalTablePageVM(journal, ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
        }
    }
}
