using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Workspace.LabsTab
{
    public partial class LabsTabPage : Page
    {
        public LabsTabPage(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
        {
            InitializeComponent();
            DataContext = new LabsTabPageVM(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
        }

        private void TreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            (DataContext as LabsTabPageVM).OpenNewJournal((e.NewValue as Node).IdJournal);
        }
    }
}
