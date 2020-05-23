using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Workspace
{
    public partial class WorkspacePage : Page
    {
        public WorkspacePage(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
        {
            InitializeComponent();
            DataContext = new WorkspacePageVM(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
        }
    }
}