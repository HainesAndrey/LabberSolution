using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace LabberClient.Students.StudentsTable
{
    public partial class StudentsTablePage : Page
    {
        public StudentsTablePage(ObservableCollection<Group> groups, List<Student> students, ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new StudentsTablePageVM(groups, students, responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }
    }
}
