using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Subjects
{
    public partial class AddSubjectsPage : Page
    {
        public AddSubjectsPage(ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new AddSubjectsPageVM(responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }
    }
}
