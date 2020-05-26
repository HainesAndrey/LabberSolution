using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Workspace.AdminTab
{
    public partial class AdminTabPage : Page
    {
        public AdminTabPage(ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new AdminTabPageVM(responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }
    }
}
