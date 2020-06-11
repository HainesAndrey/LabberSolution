using LabberClient.CreateDB.UsersTable;
using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using MvvmCross.Commands;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabberClient.CreateDB
{
    public class CreateDBPageVM : LabberVMBase
    {
        private string fileName = "";
        private string filePath = "";
        private string fullPath = "";

        public string FullPath { get => fullPath; set { fullPath = value; DBWorker.FilePath = fullPath; RaisePropertyChanged("FullPath"); } }
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

        public override void LoadData()
        {
            
        }

        private async void NextBody()
        {
            var usersCount = (UsersTablePage.DataContext as UsersTablePageVM).Users.Count;
            if (FileName == "")
                InvokeResponseEvent(ResponseType.Bad, "Укажите название файла новой базы данных");
            else if (FullPath == "")
                InvokeResponseEvent(ResponseType.Bad, "Укажите путь к файлу новой базы данных");
            else if (usersCount == 0)
                InvokeResponseEvent(ResponseType.Bad, "Добавьте пользователей базы данных");
            //else if (!Users.Any(x => x.IsAdmin))
            //    InvokeResponseEvent(ResponseType.Bad, "Добавьте хотя бы одного администратора");
            else
            {
                InvokePageEnabledEvent(false);
                InvokeLoadingStateEvent(true);
                await Task.Run(() =>
                {
                    DBWorker.FilePath = FullPath;
                    using (db = new DBWorker())
                    {
                        DBWorker.UserId = db.Users.First(x => x.Login == DBWorker.CredName).Id;
                    }
                });
                InvokeLoadingStateEvent(false);
                InvokePageEnabledEvent(true);
                InvokeCompleteStateEvent("next");
            }
        }

        private void CancelBody()
        {
            if (DBWorker.FilePath != "")
                try
                {
                    using (db = new DBWorker())
                    {
                        db.DisconnectAndDelete();
                    }
                }
                catch (FileNotFoundException)
                {
                    DBWorker.FilePath = "";
                }
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
                    if (File.Exists($"{folderDialog.SelectedPath}\\{FileName}.db"))
                        InvokeResponseEvent(ResponseType.Bad, "Файл базы данных в данном каталоге уже существует");
                    else
                        FilePath = folderDialog.SelectedPath;
                    break;
            }
        }
    }
}