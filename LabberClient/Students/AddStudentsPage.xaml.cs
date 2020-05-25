using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Students
{
    public partial class AddStudentsPage : Page
    {
        public AddStudentsPage(ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new AddStudentsPageVM(responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }
    }
}
