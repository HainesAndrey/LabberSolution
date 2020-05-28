using LabberClient.VMStuff;
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
    }
}
