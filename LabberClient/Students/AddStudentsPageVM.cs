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
            db?.Disconnect();
            InvokeCompleteStateEvent("cancel");
        }

        private void NextBody()
        {
            using (db = new DBWorker())
            {
                db.Groups.AddRange((StudentsTablePage.DataContext as StudentsTablePageVM).Groups);
                db.Students.AddRange((StudentsTablePage.DataContext as StudentsTablePageVM).Items);
                db.SaveChanges();
            }
            InvokeCompleteStateEvent("next");
        }
    }
}
