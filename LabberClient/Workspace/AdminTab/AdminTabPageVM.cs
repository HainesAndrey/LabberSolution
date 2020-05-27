using LabberClient.VMStuff;
using LabberClient.Workspace.AdminTab.JournalsCreater;
using LabberClient.Workspace.AdminTab.StudentsTab;
using LabberClient.Workspace.AdminTab.SubjectsTab;
using LabberClient.Workspace.AdminTab.UsersTab;

namespace LabberClient.Workspace.AdminTab
{
    public class AdminTabPageVM : LabberVMBase
    {
        public UsersTabPage UsersTabPage { get; set; }
        public SubjectsTabPage SubjectsTabPage { get; set; }
        public StudentsTabPage StudentsTabPage { get; set; }
        public JournalsCreaterPage JournalsCreaterPage { get; set; }
        //public int ProfilePage { get; set; }

        public AdminTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            JournalsCreaterPage = new JournalsCreaterPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
            StudentsTabPage = new StudentsTabPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
            SubjectsTabPage = new SubjectsTabPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
            UsersTabPage = new UsersTabPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
        }
    }
}
