using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using Microsoft.Win32;
using MvvmCross.Commands;
using System.Linq;

namespace LabberClient.Login
{
    public class LoginPageVM : LabberVMBase
    {
        //private string filePath;
        private string fileName = "\"Не выбран\"";
        //private string name = Environment.UserName;
        private string login = "adminPOIT";

        //public string FilePath { get => filePath; set { filePath = value; OnPropertyChanged("FilePath"); } }
        public string FileName { get => fileName; set { fileName = value; RaisePropertyChanged("FileName"); } }
        public MvxCommand ShowFileDialog { get; private set; }
        public MvxCommand<string> LogIn { get; private set; }
        public string Login { get => login; set { login = value; RaisePropertyChanged("Login"); } }

        public LoginPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            ShowFileDialog = new MvxCommand(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    InitialDirectory = "shell:MyComputerFolder",
                    DefaultExt = ".db",
                    Title = "Выберите файл базы данных",
                    Filter = "Файл базы данных|*.db"
                };
                if ((bool)openFileDialog.ShowDialog())
                    FileName = openFileDialog.SafeFileName;
                else
                    FileName = "\"Не выбран\"";
                DBWorker.FilePath = openFileDialog.FileName;
            });

            LogIn = new MvxCommand<string>(LogInAction);

        }

        private void LogInAction(string psw)
        {
            if (FileName == "\"Не выбран\"")
            {
                if (DBWorker.CredName == Login && DBWorker.CredPsw == psw)
                    InvokeCompleteStateEvent("createDB");
            }
            else
                DBWorker.UserId = db.Users.FirstOrDefault(x => x.Login == Login && x.Password == psw)?.Id ?? 0;
        }

    }
}