using LabberClient.CreateDB;
using LabberClient.Login;
using LabberClient.Students;
using LabberClient.Subjects;
using LabberClient.VMStuff;
using LabberClient.Workspace;
using LabberClient.Workspace.AdminTab;
using MvvmCross.ViewModels;
using System.Windows.Controls;
using System.Windows.Media;

namespace LabberClient
{
    public class MainWindowVM : MvxViewModel, ILabberVM
    {
        private Page currentPage;
        private string responseMessage = "";
        private Brush responseBrush;
        private bool pageEnabledState = true;
        private bool loadingState = false;
        private CreateDBPage createDBPage;
        private AddSubjectsPage addSubjectsPage;
        private AddStudentsPage addStudentsPage;
        private WorkspacePage workspacePage;
        private AdminTabPage adminTabPage;

        public event ResponseHandler ResponseEvent;
        public event PageEnabledHandler PageEnabledEvent;
        public event LoadingStateHandler LoadingStateEvent;
        public event CompleteStateHanlder CompleteStateEvent;

        public Page CurrentPage { get => currentPage; set { currentPage = value; RaisePropertyChanged("CurrentPage"); } }
        public Brush ResponseBrush { get => responseBrush; set { responseBrush = value; RaisePropertyChanged("ResponseBrush"); } }
        public bool PageEnabledState { get => pageEnabledState; set { pageEnabledState = value; RaisePropertyChanged("PageEnabledState"); } }
        public bool LoadingState { get => loadingState; set { loadingState = value; RaisePropertyChanged("LoadingState"); } }
        public string ResponseMessage { get => responseMessage; set { responseMessage = value; RaisePropertyChanged("ResponseMessage"); }}

        public MainWindowVM()
        {
            ResponseEvent += MainWindowVM_ResponseEvent;
            PageEnabledEvent += MainWindowVM_PageEnabledEvent;
            LoadingStateEvent += MainWindowVM_LoadingStateEvent;
            CompleteStateEvent += MainWindowVM_CompleteStateEvent;

            CurrentPage = new LoginPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
        }

        private void MainWindowVM_CompleteStateEvent(string parameter)
        {
            switch (CurrentPage.GetType().Name)
            {
                case nameof(LoginPage):
                    switch (parameter)
                    {
                        case "createDB":
                            createDBPage = new CreateDBPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                            addSubjectsPage = new AddSubjectsPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                            addStudentsPage = new AddStudentsPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                            CurrentPage = createDBPage;
                            break;
                        case "next":
                            if (workspacePage == null)
                                workspacePage = new WorkspacePage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                            CurrentPage = workspacePage;
                            break;
                    }
                    break;

                case nameof(CreateDBPage):
                    switch (parameter)
                    {
                        case "cancel":
                            CurrentPage = new LoginPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                            break;

                        case "next":
                            CurrentPage = addSubjectsPage;
                            break;
                    }   
                    break;

                case nameof(AddSubjectsPage):
                    switch (parameter)
                    {
                        case "cancel":
                            CurrentPage = createDBPage;
                            break;

                        case "next":
                            CurrentPage = addStudentsPage;
                            break;
                    }
                    break;

                case nameof(AddStudentsPage):
                    switch (parameter)
                    {
                        case "cancel":
                            CurrentPage = addSubjectsPage;
                            break;

                        case "next":
                            if (workspacePage == null)
                                workspacePage = new WorkspacePage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                            CurrentPage = workspacePage;
                            break;
                    }
                    break;

                case nameof(WorkspacePage):
                    switch (parameter)
                    {
                        case "admintab":
                            if (adminTabPage == null)
                                adminTabPage = new AdminTabPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                            CurrentPage = adminTabPage;
                            break;
                    }
                    break;

                case nameof(AdminTabPage):
                    switch (parameter)
                    {
                        case "workspace":
                            if (workspacePage == null)
                                workspacePage = new WorkspacePage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                            CurrentPage = workspacePage;
                            break;
                    }
                    break;
            }   
        }

        private void MainWindowVM_LoadingStateEvent(bool state)
        {
            LoadingState = state;
        }

        private void MainWindowVM_PageEnabledEvent(bool state)
        {
            PageEnabledState = state;
        }

        private void MainWindowVM_ResponseEvent(ResponseType responseType, string msg)
        {
            ResponseMessage = msg;
            switch (responseType)
            {
                case ResponseType.Neutral:
                    ResponseBrush = Brushes.DodgerBlue;
                    break;

                case ResponseType.Good:
                    ResponseBrush = Brushes.Green;
                    break;

                case ResponseType.Bad:
                    ResponseBrush = Brushes.Red;
                    break;

                default:
                    ResponseBrush = Brushes.Black;
                    break;
            }
        }
    }
}