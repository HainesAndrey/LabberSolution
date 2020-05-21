using LabberClient.VMStuff;
using LabberClient.CreateDB;
using LabberClient.Workspace;
using LabberClient.Login;
using System.Windows.Controls;
using System.Windows.Media;

namespace LabberClient
{
    public class MainWindowVM : VMBase, ILabberVM
    {
        private Page currentPage;
        private string responseMessage;
        private Brush responseBrush;
        private bool pageEnabledState;
        private bool loadingState;

        public event ResponseHandler ResponseEvent;
        public event PageEnabledHandler PageEnabledEvent;
        public event LoadingStateHandler LoadingStateEvent;
        public event CompleteStateHanlder CompleteStateEvent;

        public Page CurrentPage { get => currentPage; set { currentPage = value; OnPropertyChanged("CurrentPage"); } }
        public string ResponseMessage { get => responseMessage; set { responseMessage = value; OnPropertyChanged("ResponseMessage"); } }
        public Brush ResponseBrush { get => responseBrush; set { responseBrush = value; OnPropertyChanged("ResponseBrush"); } }
        public bool PageEnabledState { get => pageEnabledState; set { pageEnabledState = value; OnPropertyChanged("PageEnabledState"); } }
        public bool LoadingState { get => loadingState; set { loadingState = value; OnPropertyChanged("LoadingState"); } }

        public MainWindowVM()
        {
            ResponseEvent += MainWindowVM_ResponseEvent;
            PageEnabledEvent += MainWindowVM_PageEnabledEvent;
            LoadingStateEvent += MainWindowVM_LoadingStateEvent;
            CompleteStateEvent += MainWindowVM_CompleteStateEvent;

            CurrentPage = new LoginPage(0, null, ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
        }

        private void MainWindowVM_CompleteStateEvent(object parameter)
        {
            switch (CurrentPage.GetType().Name)
            {
                case nameof(LoginPage):
                    if (parameter is string && (string)parameter == "createDB")
                        CurrentPage = new CreateDBPage(0, null, ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                    break;

                case nameof(CreateDBPage):
                    if (parameter is string && (string)parameter == "cancel")
                        CurrentPage = new LoginPage(0, null, ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
                    break;

                default:
                    CurrentPage = new WorkspacePage((uint)parameter, Settings.Default.dbconnectionstring, ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
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