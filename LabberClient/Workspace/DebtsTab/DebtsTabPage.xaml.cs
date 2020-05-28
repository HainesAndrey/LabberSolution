using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Workspace.DebtsTab
{
    public partial class DebtsTabPage : Page
    {
        public DebtsTabPage(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
        {
            InitializeComponent();
            DataContext = new DebtsTabPageVM(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
        }
    }
}
