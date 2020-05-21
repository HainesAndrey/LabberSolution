using LabberClient.VMStuff;

namespace LabberClient.Subjects
{
    public class AddSubjectsPageVM : LabberVMBase
    {
        public AddSubjectsPageVM(uint userId, string dbconnectionstring, ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(userId, dbconnectionstring, ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {

        }
    }
}
