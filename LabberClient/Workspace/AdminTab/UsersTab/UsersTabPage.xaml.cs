using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Workspace.AdminTab.UsersTab
{
    public partial class UsersTabPage : Page
    {
        public UsersTabPage(ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new UsersTabPageVM(responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }
    }
}
