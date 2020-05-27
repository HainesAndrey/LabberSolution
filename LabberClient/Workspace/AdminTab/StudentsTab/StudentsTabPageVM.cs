using LabberClient.Students.StudentsTable;
using LabberClient.VMStuff;

namespace LabberClient.Workspace.AdminTab.StudentsTab
{
    public class StudentsTabPageVM : LabberVMBase
    {
        public StudentsTablePage StudentsTablePage { get; set; }

        public StudentsTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            //StudentsTablePage = new StudentsTablePage();
        }
    }
}
