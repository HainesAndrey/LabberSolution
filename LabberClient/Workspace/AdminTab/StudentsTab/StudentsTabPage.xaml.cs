using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Workspace.AdminTab.StudentsTab
{
    public partial class StudentsTabPage : Page
    {
        public StudentsTabPage(ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new StudentsTabPageVM(responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }
    }
}
