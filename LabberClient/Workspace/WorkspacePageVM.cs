using LabberClient.VMStuff;
using LabberClient.Workspace.AdminTab;
using LabberClient.Workspace.JournalsTab;
using LabberLib.DataBaseContext;
using System.Linq;
using System.Windows;

namespace LabberClient.Workspace
{
    public class WorkspacePageVM : LabberVMBase
    {
        private Visibility labsTabPageVisibility;
        private Visibility adminTabPageVisibility;

        public JournalsTabPage JournalsTabPage { get; set; }
        public int LabsTabPage { get; set; }
        public int DebtsTabPage { get; set; }
        public AdminTabPage AdminTabPage { get; set; }

        public Visibility LabsTabPageVisibility { get => labsTabPageVisibility; set { labsTabPageVisibility = value; RaisePropertyChanged("LabsTabPageVisibility"); } }
        public Visibility AdminTabPageVisibility { get => adminTabPageVisibility; set { adminTabPageVisibility = value; RaisePropertyChanged("AdminTabPageVisibility"); } }

        public WorkspacePageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            JournalsTabPage = new JournalsTabPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
            AdminTabPage = new AdminTabPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);

            InvokeResponseEvent(ResponseType.Good, "Добро пожаловать");

            using (db = new DBWorker())
            {
                var isAdmin = db.Users.FirstOrDefault(x => x.Id == DBWorker.UserId).RoleId == 1;
                LabsTabPageVisibility = isAdmin ? Visibility.Collapsed : Visibility.Visible;
                AdminTabPageVisibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}