using LabberClient.VMStuff;
using LabberClient.Workspace.AdminTab.JournalsCreater;
using System;
using System.Collections.Generic;
using System.Text;

namespace LabberClient.Workspace.AdminTab
{
    public class AdminTabPageVM : LabberVMBase
    {
        public int UsersTablePage { get; set; }
        public int SubjectsTablePage { get; set; }
        public int StudentsTablePage { get; set; }
        public JournalsCreaterPage JournalsCreaterPage { get; set; }
        public int ProfilePage { get; set; }

        public AdminTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            JournalsCreaterPage = new JournalsCreaterPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
        }
    }
}
