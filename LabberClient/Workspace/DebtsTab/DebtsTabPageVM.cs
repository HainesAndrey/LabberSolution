using LabberClient.VMStuff;
using LabberClient.Workspace.JournalsTab.JournalsSelector;

namespace LabberClient.Workspace.DebtsTab
{
    public class DebtsTabPageVM : LabberVMBase
    {
        public JournalsSelectorPage JournalsSelector { get; set; }

        public DebtsTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            JournalsSelector = new JournalsSelectorPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
        }
    }
}
