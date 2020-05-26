using LabberClient.VMStuff;
using System;
using System.Collections.Generic;
using System.Text;

namespace LabberClient.Workspace.AdminTab
{
    public class AdminTabPageVM : LabberVMBase
    {
        public AdminTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {

        }
    }
}
