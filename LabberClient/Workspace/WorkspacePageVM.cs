using LabberClient.VMStuff;

namespace LabberClient.Workspace
{
    public class WorkspacePageVM : LabberVMBase
    {
        public WorkspacePageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {

        }
    }
}