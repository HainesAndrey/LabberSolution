using LabberClient.VMStuff;

namespace LabberClient.Workspace
{
    public class WorkspacePageVM : LabberVMBase
    {
        public WorkspacePageVM(
            uint userId,
            string dbconnectionstring,
            ResponseHandler ResponseEvent,
            PageEnabledHandler PageEnabledEvent,
            LoadingStateHandler LoadingStateEvent,
            CompleteStateHanlder CompleteStateEvent)
            : base(userId, dbconnectionstring, ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            db.UserId = userId;
        }
    }
}