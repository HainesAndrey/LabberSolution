using LabberClient.VMStuff;
using LabberClient.Workspace.AdminTab.JournalsCreater;
using LabberClient.Workspace.AdminTab.StudentsTab;
using LabberClient.Workspace.AdminTab.SubjectsTab;
using LabberClient.Workspace.AdminTab.UsersTab;
using MvvmCross.Commands;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace LabberClient.Workspace.AdminTab
{
    public class AdminTabPageVM : LabberVMBase
    {
        private bool pageEnabledState = true;
        private Page currentPage;
        private Brush openWorkspacePageColor = Brushes.Black;
        private Brush openUsersTabPageColor = Brushes.DodgerBlue;
        private Brush openSubjectsTabColor = Brushes.Black;
        private Brush openStudentsTabColor = Brushes.Black;
        private Brush openJournalsCreaterColor = Brushes.Black;

        public bool PageEnabledState { get => pageEnabledState; set { pageEnabledState = value; RaisePropertyChanged("PageEnabledState"); } }
        public Brush OpenWorkspacePageColor { get => openWorkspacePageColor; set { openWorkspacePageColor = value; RaisePropertyChanged("OpenWorkspacePageColor"); } }
        public Brush OpenUsersTabPageColor { get => openUsersTabPageColor; set { openUsersTabPageColor = value; RaisePropertyChanged("OpenUsersTabPageColor"); } }
        public Brush OpenSubjectsTabColor { get => openSubjectsTabColor; set { openSubjectsTabColor = value; RaisePropertyChanged("OpenSubjectsTabColor"); } }
        public Brush OpenStudentsTabColor { get => openStudentsTabColor; set { openStudentsTabColor = value; RaisePropertyChanged("OpenStudentsTabColor"); } }
        public Brush OpenJournalsCreaterColor { get => openJournalsCreaterColor; set { openJournalsCreaterColor = value; RaisePropertyChanged("OpenJournalsCreaterColor"); } }
        public Page CurrentPage { get => currentPage; set { currentPage = value; RaisePropertyChanged("CurrentPage"); } }

        public UsersTabPage UsersTabPage { get; set; }
        public SubjectsTabPage SubjectsTabPage { get; set; }
        public StudentsTabPage StudentsTabPage { get; set; }
        public JournalsCreaterPage JournalsCreaterPage { get; set; }

        public MvxCommand OpenWorkspacePage { get; set; }
        public MvxCommand OpenUsersTabPage { get; set; }
        public MvxCommand OpenSubjectsTabPage { get; set; }
        public MvxCommand OpenStudentsTabPage { get; set; }
        public MvxCommand OpenJournalsCreater { get; set; }

        public AdminTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            JournalsCreaterPage = new JournalsCreaterPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
            StudentsTabPage = new StudentsTabPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
            SubjectsTabPage = new SubjectsTabPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
            UsersTabPage = new UsersTabPage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);

            CurrentPage = UsersTabPage;

            OpenWorkspacePage = new MvxCommand(OpenWorkspacePageBody);
            OpenUsersTabPage = new MvxCommand(OpenUsersTabPageBody);
            OpenSubjectsTabPage = new MvxCommand(OpenSubjectsTabPageBody);
            OpenStudentsTabPage = new MvxCommand(OpenStudentsTabPageBody);
            OpenJournalsCreater = new MvxCommand(OpenJournalsCreaterBody);
        }

        private void OpenJournalsCreaterBody()
        {
            CurrentPage = JournalsCreaterPage;
            OpenWorkspacePageColor = Brushes.Black;
            OpenUsersTabPageColor = Brushes.Black;
            OpenSubjectsTabColor = Brushes.Black;
            OpenStudentsTabColor = Brushes.Black;
            OpenJournalsCreaterColor = Brushes.DodgerBlue;
        }

        private void OpenStudentsTabPageBody()
        {
            CurrentPage = StudentsTabPage;
            OpenWorkspacePageColor = Brushes.Black;
            OpenUsersTabPageColor = Brushes.Black;
            OpenSubjectsTabColor = Brushes.Black;
            OpenStudentsTabColor = Brushes.DodgerBlue;
            OpenJournalsCreaterColor = Brushes.Black;
        }

        private void OpenSubjectsTabPageBody()
        {
            CurrentPage = SubjectsTabPage;
            OpenWorkspacePageColor = Brushes.Black;
            OpenUsersTabPageColor = Brushes.Black;
            OpenSubjectsTabColor = Brushes.DodgerBlue;
            OpenStudentsTabColor = Brushes.Black;
            OpenJournalsCreaterColor = Brushes.Black;
        }

        private void OpenUsersTabPageBody()
        {
            CurrentPage = UsersTabPage;
            OpenWorkspacePageColor = Brushes.Black;
            OpenUsersTabPageColor = Brushes.DodgerBlue;
            OpenSubjectsTabColor = Brushes.Black;
            OpenStudentsTabColor = Brushes.Black;
            OpenJournalsCreaterColor = Brushes.Black;
        }

        private void OpenWorkspacePageBody()
        {
            InvokeCompleteStateEvent("workspace");
        }
    }
}
