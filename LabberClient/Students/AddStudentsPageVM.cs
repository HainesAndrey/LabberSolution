using LabberClient.Students.StudentsTable;
using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using MvvmCross.Commands;

namespace LabberClient.Students
{
    public class AddStudentsPageVM : LabberVMBase
    {
        public StudentsTablePage StudentsTablePage { get; set; }

        public MvxCommand Cancel { get; set; }
        public MvxCommand Next { get; set; }

        public AddStudentsPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            StudentsTablePage = new StudentsTablePage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);

            Next = new MvxCommand(NextBody);
            Cancel = new MvxCommand(CancelBody);
        }

        private void CancelBody()
        {
            using (db = new DBWorker())
            {
                db.Disconnect();
            }
            InvokeCompleteStateEvent("cancel");
        }

        private void NextBody()
        {
            InvokeCompleteStateEvent("next");
        }
    }
}
