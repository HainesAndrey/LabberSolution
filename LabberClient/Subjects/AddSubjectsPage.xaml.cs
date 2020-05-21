using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Subjects
{
    public partial class AddSubjectsPage : Page
    {
        public AddSubjectsPage(
            uint userId,
            string dbconnectionstring,
            ResponseHandler responseEvent,
            PageEnabledHandler pageEnabledEvent,
            LoadingStateHandler loadingStateEvent,
            CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new AddSubjectsPageVM(userId, dbconnectionstring, responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }
    }
}
