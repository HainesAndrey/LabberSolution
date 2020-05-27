using LabberClient.Subjects.SubjectsTable;
using LabberClient.VMStuff;

namespace LabberClient.Workspace.AdminTab.SubjectsTab
{
    public class SubjectsTabPageVM : LabberVMBase
    {
        public SubjectsTablePage SubjectsTablePage{ get; set; }

        public SubjectsTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            SubjectsTablePage = new SubjectsTablePage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
        }
    }
}
