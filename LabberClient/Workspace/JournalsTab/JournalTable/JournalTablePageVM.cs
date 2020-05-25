using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;

namespace LabberClient.Workspace.JournalsTab.JournalTable
{
    public class JournalTablePageVM : LabberVMBase
    {
        public Journal Journal { get; set; }

        public JournalTablePageVM(Journal journal, ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            Journal = journal;
        }
    }
}
