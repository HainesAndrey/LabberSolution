using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Login
{
    public partial class LoginPage : Page
    {
        public LoginPage(
            uint userId,
            string dbconnectionstring,
            ResponseHandler responseEvent,
            PageEnabledHandler pageEnabledEvent,
            LoadingStateHandler loadingStateEvent,
            CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new LoginPageVM(userId, dbconnectionstring, responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as LoginPageVM).LogIn.Execute(psw.Password);
        }
    }
}
