using LabberClient.Students.StudentsTable;
using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using MvvmCross.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LabberClient.Students
{
    public class AddStudentsPageVM : LabberVMBase
    {
        public StudentsTablePage StudentsTablePage { get; set; }
        public List<Student> Students { get; set; } = new List<Student>();
        public ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group>();
        public MvxCommand Cancel { get; set; }
        public MvxCommand Next { get; set; }

        public AddStudentsPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            StudentsTablePage = new StudentsTablePage(Groups, Students, InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);

            Next = new MvxCommand(NextBody);
            Cancel = new MvxCommand(CancelBody);
        }

        private void CancelBody()
        {
            db?.Disconnect();
            InvokeCompleteStateEvent("cancel");
        }

        private void NextBody()
        {
            db = new DBWorker();
            db.Groups.AddRange(Groups);
            db.Students.AddRange(Students);
            db.SaveChanges();
        }
    }
}
