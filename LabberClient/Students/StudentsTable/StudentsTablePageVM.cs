using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;
using System.Collections.ObjectModel;

namespace LabberClient.Students.StudentsTable
{
    public class StudentsTablePageVM : LabberVMBase
    {
        public StudentsTablePageVM(ObservableCollection<Student> students, ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {

        }
    }
}
