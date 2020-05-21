using GalaSoft.MvvmLight.CommandWpf;
using LabberClient.VMStuff;
using Microsoft.Win32;
using System.Linq;
using System.Windows;

namespace LabberClient.Login
{
    public class LoginPageVM : LabberVMBase
    {
        //private string filePath;
        private string fileName = "\"Не выбран\"";
        //private string name = Environment.UserName;
        private string login = "adminPOIT";

        //public string FilePath { get => filePath; set { filePath = value; OnPropertyChanged("FilePath"); } }
        public string FileName { get => fileName; set { fileName = value; OnPropertyChanged("FileName"); } }
        public RelayCommand ShowFileDialog { get; private set; }
        public RelayCommand<string> LogIn { get; private set; }
        public string Login { get => login; set { login = value; OnPropertyChanged("Login"); } }

        public LoginPageVM(uint userId, string dbconnectionstring, ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(userId, dbconnectionstring, ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            ShowFileDialog = new RelayCommand(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    InitialDirectory = "shell:MyComputerFolder",
                    DefaultExt = ".sqlite",
                    Title = "Выберите файл базы данных",
                    Filter = "Файл базы данных|*.sqlite"
                };
                if ((bool)openFileDialog.ShowDialog())
                    FileName = openFileDialog.SafeFileName;
                else
                    FileName = "\"Не выбран\"";
                db.FilePath = openFileDialog.FileName;
                Settings.Default.dbconnectionstring = openFileDialog.FileName;
            });

            LogIn = new RelayCommand<string>(LogInAction);

        }

        private void LogInAction(string psw)
        {
            if (db.FilePath is null || db.FilePath == "")
            {
                if (db.CredName == Login && db.CredPsw == psw)
                    InvokeCompleteStateEvent("createDB");
            }
            else
                db.UserId = db.Users.FirstOrDefault(x => x.Login == Login && x.Password == psw)?.Id ?? 0;
        }

    }
}