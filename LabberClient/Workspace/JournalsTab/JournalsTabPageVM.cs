using LabberClient.VMStuff;

namespace LabberClient.Workspace.JournalsTab
{
    public class JournalsTabPageVM : LabberVMBase
    {
        public JournalsTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {

        }
    }
}
