using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Subjects.SubjectsTable
{
    public partial class SubjectsTablePage : Page
    {
        public SubjectsTablePage(ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new SubjectsTablePageVM(responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }
    }
}
