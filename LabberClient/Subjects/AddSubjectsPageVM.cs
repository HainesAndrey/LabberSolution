using LabberClient.Subjects.SubjectsTable;
using LabberClient.VMStuff;
using MvvmCross.Commands;
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
                InvokeCompleteStateEvent("cancel");
            });

            Next = new MvxCommand(async () =>
            {
                if ((SubjectsTablePage.DataContext as SubjectsTablePageVM).Items.Count != 0)
                {
                    InvokeLoadingStateEvent(true);
                    await Task.Run(() =>
                    {
                        db.Subjects.AddRange((SubjectsTablePage.DataContext as SubjectsTablePageVM).Items);
                        db.SaveChanges();
                        InvokeResponseEvent(ResponseType.Good, "Дисциплины успешно добавлены");
                    });
                    InvokeLoadingStateEvent(false);
                }
                InvokeCompleteStateEvent("next");
            });
        }
    }
}
