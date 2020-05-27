using LabberClient.Subjects.SubjectsTable;
using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using MvvmCross.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LabberClient.Subjects
{
    public class AddSubjectsPageVM : LabberVMBase
    {
        public SubjectsTablePage SubjectsTablePage { get; set; }

        public MvxCommand Cancel { get; set; }
        public MvxCommand Next { get; set; }

        public AddSubjectsPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            SubjectsTablePage = new SubjectsTablePage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);

            Cancel = new MvxCommand(() =>
            {
                using (db = new DBWorker())
                {
                    db?.Disconnect();
                }
                InvokeCompleteStateEvent("cancel");
            });

            Next = new MvxCommand(() => InvokeCompleteStateEvent("next"));
        }
    }
}
