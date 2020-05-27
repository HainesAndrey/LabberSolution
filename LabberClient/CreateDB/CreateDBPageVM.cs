using LabberClient.CreateDB.UsersTable;
using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using MvvmCross.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;

namespace LabberClient.CreateDB
{
    public class CreateDBPageVM : LabberVMBase
    {
        private string fileName = "";
        private string filePath = "";
        private string fullPath = "";

        public string FullPath { get => fullPath; set { fullPath = value; RaisePropertyChanged("FullPath"); } }
        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value;
                RaisePropertyChanged("FileName");
                if (FilePath != "")
                    FullPath = $"{FilePath}\\{fileName}{(fileName != "" ? ".db" : "")}";
            }
        }
        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                RaisePropertyChanged("FilePath");
                if (FileName != "")
                    FullPath = $"{filePath}\\{FileName}.db";
                else
                    FullPath = filePath;
            }
        }

        public ObservableCollection<UserDTO> Users { get; set; }
        public MvxCommand ShowFileDialog { get; set; }
        public MvxCommand Cancel { get; set; }
        public MvxCommand Next { get; set; }

        public UsersTablePage UsersTablePage { get; set; }

        public CreateDBPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            ShowFileDialog = new MvxCommand(ShowFileDialogBody);
            Cancel = new MvxCommand(CancelBody);
            Next = new MvxCommand(NextBody);

            UsersTablePage = new UsersTablePage(InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent);
        }

        private async void NextBody()
        {
            Users = (UsersTablePage.DataContext as UsersTablePageVM).Users;
            if (FileName == "")
                InvokeResponseEvent(ResponseType.Bad, "Укажите название файла новой базы данных");
            else if (FilePath == "")
                InvokeResponseEvent(ResponseType.Bad, "Укажите путь к файлу новой базы данных");
            else if (Users.Count == 0)
                InvokeResponseEvent(ResponseType.Bad, "Добавьте пользователей базы данных");
            else if (!Users.Any(x => x.IsAdmin))
                InvokeResponseEvent(ResponseType.Bad, "Добавьте хотя бы одного администратора");
            else
            {
                InvokeResponseEvent(ResponseType.Neutral, "Подождите...");
                InvokePageEnabledEvent(false);
                InvokeLoadingStateEvent(true);
                await Task.Run(() =>
                {
                    DBWorker.FilePath = FullPath;
                    db = new DBWorker(true);
                    
                    db.Users.Add(new User(1, DBWorker.CredName, "Admin", "ПОИТ") { Password = DBWorker.CredPsw });
                    db.Users.AddRange(Users.Select(x => x.User).Where(x => !db.Users.ToList().Exists(y => y.Login == x.Login)));
                    db.SaveChanges();

                    DBWorker.UserId = db.Users.First(x => x.Login == DBWorker.CredName).Id;

                    InvokeResponseEvent(ResponseType.Good, "База данных успешно создана. Пользователи добавлены в базу данных");
                });
                InvokeLoadingStateEvent(false);
                InvokePageEnabledEvent(true);
                InvokeCompleteStateEvent("next");
            }
        }

        private void CancelBody()
        {
            db?.DisconnectAndDelete();
            InvokeCompleteStateEvent("cancel");
        }

        private void ShowFileDialogBody()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog()
            {
                RootFolder = System.Environment.SpecialFolder.MyComputer,
                UseDescriptionForTitle = true,
                Description = "Выберите путь для файла новой базы данных"
            };
            switch (folderDialog.ShowDialog())
            {
                case DialogResult.OK:
                    FilePath = folderDialog.SelectedPath;
                    break;
            }
        }
    }
}