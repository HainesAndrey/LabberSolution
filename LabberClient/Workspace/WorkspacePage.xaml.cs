using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Workspace
{
    public partial class WorkspacePage : Page
    {
        public WorkspacePage(
            uint userId,
            string dbconnectionstring,
            ResponseHandler ResponseEvent,
            PageEnabledHandler PageEnabledEvent,
            LoadingStateHandler LoadingStateEvent,
            CompleteStateHanlder CompleteStateEvent)
        {
            InitializeComponent();
            DataContext = new WorkspacePageVM(userId, dbconnectionstring, ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
        }
    }
}