using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Login
{
    public partial class LoginPage : Page
    {
        public LoginPage(ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new LoginPageVM(responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as LoginPageVM).LogIn.Execute(psw.Password);
        }
    }
}
