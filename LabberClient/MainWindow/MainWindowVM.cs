using LabberClient.VMStuff;
using LabberClient.CreateDB;
using LabberClient.Workspace;
using LabberClient.Login;
using System.Windows.Controls;
using System.Windows.Media;
using MvvmCross.ViewModels;
using LabberClient.Subjects;
using System.Windows;
using System;
using LabberClient.Students;
using LabberClient.Workspace.AdminTab;

namespace LabberClient
{
    public class MainWindowVM : MvxViewModel, ILabberVM
    {
        private Page currentPage;
        private string responseMessage = "";
        private Duration duration = new Duration(new TimeSpan(0, 0, 5));
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
        public Duration Duration { get => duration; set { duration = value; RaisePropertyChanged("Duration"); } }
        public Brush ResponseBrush { get => responseBrush; set { responseBrush = value; RaisePropertyChanged("ResponseBrush"); } }
        public bool PageEnabledState { get => pageEnabledState; set { pageEnabledState = value; RaisePropertyChanged("PageEnabledState"); } }
        public bool LoadingState { get => loadingState; set { loadingState = value; RaisePropertyChanged("LoadingState"); } }
        public string ResponseMessage { get => responseMessage;
            set
            {
                responseMessage = value;
                Duration = GetDuration(responseMessage);
                RaisePropertyChanged("ResponseMessage");
            }
        }

        public MainWindowVM()
        {
            ResponseEvent += MainWindowVM_ResponseEvent;
            PageEnabledEvent += MainWindowVM_PageEnabledEvent;
            LoadingStateEvent += MainWindowVM_LoadingStateEvent;
            CompleteStateEvent += MainWindowVM_CompleteStateEvent;

            //createDBPage = new CreateDBPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
            //addSubjectsPage = new AddSubjectsPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);

            CurrentPage = new LoginPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
            //CurrentPage = new AddStudentsPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent); ;
            //CurrentPage = new WorkspacePage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
        }

        private void MainWindowVM_CompleteStateEvent(object parameter)
        {
            switch (CurrentPage.GetType().Name)
            {
                case nameof(LoginPage):
                    switch ((string)parameter)
                    {
                        case "createDB":
                            createDBPage = new CreateDBPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                            addSubjectsPage = new AddSubjectsPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                            addStudentsPage = new AddStudentsPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                            CurrentPage = createDBPage;
                            break;
                        case "next":
                            CurrentPage = new WorkspacePage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                            break;
                    }
                    break;

                case nameof(CreateDBPage):
                    switch ((string)parameter)
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
                    switch ((string)parameter)
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
                    switch ((string)parameter)
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
                    switch ((string)parameter)
                    {
                        case "admintab":
                            if (adminTabPage == null)
                                adminTabPage = new AdminTabPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                            CurrentPage = adminTabPage;
                            break;
                    }
                    break;

                case nameof(AdminTabPage):
                    switch ((string)parameter)
                    {
                        case "workspace":
                            if (workspacePage == null)
                                workspacePage = new WorkspacePage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                            CurrentPage = workspacePage;
                            break;
                    }
                    break;

                    //default:
                    //    CurrentPage = new WorkspacePage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                    //    break;
            }   
        }

        private Duration GetDuration(string value)
        {
            return new Duration(TimeSpan.FromSeconds(value.Length / 15));
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