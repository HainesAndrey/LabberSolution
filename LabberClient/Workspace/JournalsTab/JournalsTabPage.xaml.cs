using LabberClient.VMStuff;
using LabberClient.Workspace.JournalsTab.JournalTable;
using LabberLib.DataBaseContext.Entities;
using System.Windows.Controls;

namespace LabberClient.Workspace.JournalsTab
{
    public partial class JournalsTabPage : Page
    {
        public JournalsTabPage(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
        {
            InitializeComponent();
            DataContext = new JournalsTabPageVM(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
        }

        private void TreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            (DataContext as JournalsTabPageVM).OpenNewJournal((e.NewValue as Node).IdJournal);
        }
    }
}
