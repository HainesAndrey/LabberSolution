using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Workspace.AdminTab.JournalsCreater
{
    public partial class JournalsCreaterPage : Page
    {
        public JournalsCreaterPage(ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new JournalsCreaterPageVM(responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }
    }
}
