using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace LabberClient.Subjects.SubjectsTable
{
    public partial class SubjectsTablePage : Page
    {
        public SubjectsTablePage(ObservableCollection<Subject> subjects, ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new SubjectsTablePageVM(subjects, responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }
    }
}
