using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace LabberClient.Students.StudentsTable
{
    public partial class StudentsTablePage : Page
    {
        public StudentsTablePage(ObservableCollection<Student> students, ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new StudentsTablePageVM(students, responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }
    }
}
