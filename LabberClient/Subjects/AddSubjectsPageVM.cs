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
        public ObservableCollection<Subject> Subjects { get; set; } = new ObservableCollection<Subject>();
        public MvxCommand Cancel { get; set; }
        public MvxCommand Next { get; set; }

        public AddSubjectsPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            SubjectsTablePage = new SubjectsTablePage(Subjects, InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);

            Cancel = new MvxCommand(() =>
            {
                db?.Disconnect();
                InvokeCompleteStateEvent("cancel");
            });

            Next = new MvxCommand(async () =>
            {
                if ((SubjectsTablePage.DataContext as SubjectsTablePageVM).Items.Count != 0)
                {
                    InvokeResponseEvent(ResponseType.Neutral, "Подождите...");
                    InvokePageEnabledEvent(false);
                    InvokeLoadingStateEvent(true);
                    await Task.Run(() =>
                    {
                        if (db is null)
                            db = new DBWorker();
                        db.Subjects.AddRange(Subjects.Where(x => !db.Subjects.ToList().Exists(y => y.ShortTitle == x.ShortTitle)));
                        db.SaveChanges();
                        InvokeResponseEvent(ResponseType.Good, "Дисциплины успешно добавлены в базу данных");
                    });
                    InvokeLoadingStateEvent(false);
                    InvokePageEnabledEvent(true);
                }
                InvokeCompleteStateEvent("next");
            });
        }
    }
}
