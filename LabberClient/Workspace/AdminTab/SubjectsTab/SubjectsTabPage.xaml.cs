using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Workspace.AdminTab.SubjectsTab
{
    public partial class SubjectsTabPage : Page
    {
        public SubjectsTabPage(ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new SubjectsTabPageVM(responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }
    }
}
