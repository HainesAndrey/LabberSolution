using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.CreateDB
{
    public partial class CreateDBPage : Page
    {
        public CreateDBPage(
            uint userId,
            string dbconnectionstring,
            ResponseHandler responseEvent,
            PageEnabledHandler pageEnabledEvent,
            LoadingStateHandler loadingStateEvent,
            CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new CreateDBPageVM(userId, dbconnectionstring, responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }
    }
}