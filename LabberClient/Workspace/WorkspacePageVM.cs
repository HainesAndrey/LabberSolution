using LabberClient.VMStuff;
using LabberClient.Workspace.JournalsTab;

namespace LabberClient.Workspace
{
    public class WorkspacePageVM : LabberVMBase
    {
        public JournalsTabPage JournalsTabPage { get; set; }
        public int LabsTabPage { get; set; }
        public int DebtsTabPage { get; set; }
        public int AdminTabPage { get; set; }

        public WorkspacePageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            JournalsTabPage = new JournalsTabPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
        }
    }
}