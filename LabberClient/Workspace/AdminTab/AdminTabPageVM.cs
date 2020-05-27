using LabberClient.Students;
using LabberClient.Students.StudentsTable;
using LabberClient.Subjects;
using LabberClient.Subjects.SubjectsTable;
using LabberClient.VMStuff;
using LabberClient.Workspace.AdminTab.JournalsCreater;
using LabberClient.Workspace.AdminTab.UsersTable;
using System;
using System.Collections.Generic;
using System.Text;

namespace LabberClient.Workspace.AdminTab
{
    public class AdminTabPageVM : LabberVMBase
    {
        public UsersTabPage UsersTabPage { get; set; }
        public AddSubjectsPage SubjectsTablePage { get; set; }
        public AddStudentsPage StudentsTablePage { get; set; }
        public JournalsCreaterPage JournalsCreaterPage { get; set; }
        public int ProfilePage { get; set; }

        public AdminTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            JournalsCreaterPage = new JournalsCreaterPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
            StudentsTablePage = new AddStudentsPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
            SubjectsTablePage = new AddSubjectsPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
            UsersTabPage = new UsersTabPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
        }
    }
}
