using LabberClient.CreateDB.UsersTable;
using LabberClient.VMStuff;
using System;
using System.Collections.Generic;
using System.Text;

namespace LabberClient.Workspace.AdminTab.UsersTable
{
    public class UsersTabPageVM : LabberVMBase
    {
        public UsersTablePage UsersTablePage { get; set; }

        public UsersTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            UsersTablePage = new UsersTablePage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
        }
    }
}
