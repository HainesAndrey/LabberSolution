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

namespace LabberClient
{
    public class MainWindowVM : MvxViewModel, ILabberVM
    {
        private Page currentPage;
        private string responseMessage;
        private Duration duration = new Duration(new TimeSpan(0, 0, 5));
        private Brush responseBrush;
        private bool pageEnabledState;
        private bool loadingState;
        private CreateDBPage createDBPage;
        private AddSubjectsPage addSubjectsPage;

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

            createDBPage = new CreateDBPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
            addSubjectsPage = new AddSubjectsPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);

            CurrentPage = new LoginPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent); ;
        }

        private void MainWindowVM_CompleteStateEvent(object parameter)
        {
            switch (CurrentPage.GetType().Name)
            {
                case nameof(LoginPage):
                    if ((string)parameter == "createDB")
                        CurrentPage = createDBPage;
                    break;

                case nameof(CreateDBPage):
                    if ((string)parameter == "cancel")
                        CurrentPage = new LoginPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                    else if ((string)parameter == "next")
                        CurrentPage = addSubjectsPage;
                    break;

                case nameof(AddSubjectsPage):
                    if ((string)parameter == "cancel")
                        CurrentPage = createDBPage;
                    //else if ((string)parameter == "next")
                    //    CurrentPage = new AddStudentsPage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                    break;

                default:
                    CurrentPage = new WorkspacePage(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                    break;
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