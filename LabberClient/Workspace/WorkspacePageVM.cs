using LabberClient.VMStuff;
using LabberClient.Workspace.AdminTab;
using LabberClient.Workspace.DebtsTab;
using LabberClient.Workspace.JournalsTab;
using LabberClient.Workspace.LabsTab;
using LabberLib.DataBaseContext;
using MvvmCross.Commands;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LabberClient.Workspace
{
    public class WorkspacePageVM : LabberVMBase
    {
        private Visibility labsTabPageVisibility;
        private Visibility adminTabPageVisibility;
        private bool pageEnabledState = true;
        private Page currentPage;
        private Brush openJournalsTabColor = Brushes.DodgerBlue;
        private Brush openLabsTabColor = Brushes.Black;
        private Brush openDebtsTabColor = Brushes.Black;
        private Brush openAdminTabColor = Brushes.Black;

        public Visibility LabsTabPageVisibility { get => labsTabPageVisibility; set { labsTabPageVisibility = value; RaisePropertyChanged("LabsTabPageVisibility"); } }
        public Visibility AdminTabPageVisibility { get => adminTabPageVisibility; set { adminTabPageVisibility = value; RaisePropertyChanged("AdminTabPageVisibility"); } }
        public bool PageEnabledState { get => pageEnabledState; set { pageEnabledState = value; RaisePropertyChanged("PageEnabledState"); } }
        public Brush OpenJournalsTabColor { get => openJournalsTabColor; set { openJournalsTabColor = value; RaisePropertyChanged("OpenJournalsTabColor"); } }
        public Brush OpenLabsTabColor { get => openLabsTabColor; set { openLabsTabColor = value; RaisePropertyChanged("OpenLabsTabColor"); } }
        public Brush OpenDebtsTabColor { get => openDebtsTabColor; set { openDebtsTabColor = value; RaisePropertyChanged("OpenDebtsTabColor"); } }
        public Brush OpenAdminTabColor { get => openAdminTabColor; set { openAdminTabColor = value; RaisePropertyChanged("OpenAdminTabColor"); } }

        public JournalsTabPage JournalsTabPage { get; set; }
        public LabsTabPage LabsTabPage { get; set; }
        public DebtsTabPage DebtsTabPage { get; set; }
        public AdminTabPage AdminTabPage { get; set; }
        public Page CurrentPage { get => currentPage; set { currentPage = value; RaisePropertyChanged("CurrentPage"); } }

        public MvxCommand OpenJournalsTab { get; set; }
        public MvxCommand OpenLabsTab { get; set; }
        public MvxCommand OpenDebtsTab { get; set; }
        public MvxCommand OpenAdminTab { get; set; }

        public WorkspacePageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            InvokeResponseEvent(ResponseType.Good, "Добро пожаловать");

            bool isAdmin;
            using (db = new DBWorker())
            {
                isAdmin = db.Users.FirstOrDefault(x => x.Id == DBWorker.UserId).RoleId == 1;
            }
            LabsTabPageVisibility = isAdmin ? Visibility.Collapsed : Visibility.Visible;
            AdminTabPageVisibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

            JournalsTabPage = new JournalsTabPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
            DebtsTabPage = new DebtsTabPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);

            if (isAdmin)
                AdminTabPage = new AdminTabPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
            else
                LabsTabPage = new LabsTabPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);

            CurrentPage = JournalsTabPage;

            OpenJournalsTab = new MvxCommand(OpenJournalsTabBody);
            OpenDebtsTab = new MvxCommand(OpenDebtsTabBody);
            OpenLabsTab = new MvxCommand(OpenLabsTabBody);
            OpenAdminTab = new MvxCommand(OpenAdminTabBody);
        }

        private void OpenAdminTabBody()
        {
            CurrentPage = AdminTabPage;
            OpenAdminTabColor = Brushes.DodgerBlue;
            OpenDebtsTabColor = Brushes.Black;
            OpenJournalsTabColor = Brushes.Black;
            OpenLabsTabColor = Brushes.Black;
        }

        private void OpenLabsTabBody()
        {
            CurrentPage = LabsTabPage;
            OpenAdminTabColor = Brushes.Black;
            OpenDebtsTabColor = Brushes.Black;
            OpenJournalsTabColor = Brushes.Black;
            OpenLabsTabColor = Brushes.DodgerBlue;
        }

        private void OpenDebtsTabBody()
        {
            CurrentPage = DebtsTabPage;
            OpenAdminTabColor = Brushes.Black;
            OpenDebtsTabColor = Brushes.DodgerBlue;
            OpenJournalsTabColor = Brushes.Black;
            OpenLabsTabColor = Brushes.Black;
        }

        private void OpenJournalsTabBody()
        {
            CurrentPage = JournalsTabPage;
            OpenAdminTabColor = Brushes.Black;
            OpenDebtsTabColor = Brushes.Black;
            OpenJournalsTabColor = Brushes.DodgerBlue;
            OpenLabsTabColor = Brushes.Black;
        }
    }
}