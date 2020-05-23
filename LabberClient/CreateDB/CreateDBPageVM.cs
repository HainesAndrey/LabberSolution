using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;
using MvvmCross.Commands;
using OfficeOpenXml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using LicenseContext = OfficeOpenXml.LicenseContext;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace LabberClient.CreateDB
{
    public class CreateDBPageVM : LabberVMBase
    {
        private string addSaveUserBtnTitle = "Добавить";
        private string fileName = "";
        private string filePath = "";
        private bool addUserEnabled = false;
        private bool deleteUserEnabled = true;
        private bool deleteAllEnabled = false;
        private bool clearEnabled = false;
        private string login = "";
        private bool isAdmin = false;
        private string surname = "";
        private string firstName = "";
        private string secondName = "";
        private UserDTO currentUser;
        private string fullPath = "";

        public string AddSaveUserBtnTitle { get => addSaveUserBtnTitle; set { addSaveUserBtnTitle = value; RaisePropertyChanged("AddSaveUserBtnTitle"); } }
        public string FullPath { get => fullPath; set { fullPath = value; RaisePropertyChanged("FullPath"); } }
        public bool AddUserEnabled { get => addUserEnabled; set { addUserEnabled = value; RaisePropertyChanged("AddUserEnabled"); } }
        public bool DeleteUserEnabled { get => deleteUserEnabled; set { deleteUserEnabled = value; RaisePropertyChanged("DeleteUserEnabled"); } }
        public bool DeleteAllEnabled { get => deleteAllEnabled; set { deleteAllEnabled = value; RaisePropertyChanged("DeleteAllEnabled"); } }
        public bool ClearEnabled { get => clearEnabled; set { clearEnabled = value; RaisePropertyChanged("ClearEnabled"); } }
        public UserDTO CurrentUser { get => currentUser; set { currentUser = value; RaisePropertyChanged("CurrentUser"); } }
        public bool IsAdmin { get => isAdmin; set { isAdmin = value; ClearEnabled = !IsAllFieldAreEmpty(); RaisePropertyChanged("IsAdmin"); } }
        public string SecondName { get => secondName; set { secondName = value; ClearEnabled = !IsAllFieldAreEmpty(); RaisePropertyChanged("SecondName"); } }
        public string Login
        {
            get => login;
            set
            {
                login = value;
                AddUserEnabled = CheckRequiredFields();
                ClearEnabled = !IsAllFieldAreEmpty();
                RaisePropertyChanged("Login");
            }
        }
        public string Surname
        {
            get => surname;
            set
            {
                surname = value;
                AddUserEnabled = CheckRequiredFields();
                ClearEnabled = !IsAllFieldAreEmpty();
                RaisePropertyChanged("Surname");
            }
        }
        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                AddUserEnabled = CheckRequiredFields();
                ClearEnabled = !IsAllFieldAreEmpty();
                RaisePropertyChanged("FirstName");
            }
        }
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

        bool CheckRequiredFields() => Login != "" && Surname != "" && FirstName != "";
        bool IsAllFieldAreEmpty() => Login == "" && Surname == "" && FirstName == "" && SecondName == "" && !IsAdmin;

        public ObservableCollection<UserDTO> Users { get; set; }
        public MvxCommand ShowFileDialog { get; set; }
        public MvxCommand AddUser { get; set; }
        public MvxCommand ChangeUser { get; set; }
        public MvxCommand DeleteUser { get; set; }
        public MvxCommand Clear { get; set; }
        public MvxCommand DeleteAll { get; set; }
        public MvxCommand AddFromExcel { get; set; }
        public MvxCommand Cancel { get; set; }
        public MvxCommand Next { get; set; }

        public CreateDBPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            Users = new ObservableCollection<UserDTO>();
            var view = (CollectionView)CollectionViewSource.GetDefaultView(Users);
            view.SortDescriptions.Add(new SortDescription("User.Login", ListSortDirection.Ascending));

            ShowFileDialog = new MvxCommand(ShowFileDialogBody);
            AddUser = new MvxCommand(AddUserBody);
            ChangeUser = new MvxCommand(ChangeUserBody);
            AddFromExcel = new MvxCommand(AddFromExcelBody);
            DeleteUser = new MvxCommand(DeleteUserBody);
            DeleteAll = new MvxCommand(DeleteAllBody);
            Clear = new MvxCommand(ClearBody);
            Cancel = new MvxCommand(CancelBody);
            Next = new MvxCommand(NextBody);
        }

        private async void NextBody()
        {
            db.FilePath = FullPath;
            if (FileName == "")
                InvokeResponseEvent(ResponseType.Bad, "Укажите название файла новой базы данных");
            else if (FilePath == "")
                InvokeResponseEvent(ResponseType.Bad, "Укажите путь к файлу новой базы данных");
            else if (Users.Count == 0)
                InvokeResponseEvent(ResponseType.Bad, "Добавьте пользователей базы данных");
            else if (!Users.Any(x => x.IsAdmin))
                InvokeResponseEvent(ResponseType.Bad, "Добавьте хотя бы одного администратора");
            else if (!db.IsDbCreated)
            {
                InvokeResponseEvent(ResponseType.Neutral, "Подождите...");
                InvokePageEnabledEvent(false);
                InvokeLoadingStateEvent(true);
                await Task.Run(() =>
                {
                    db.Create();
                    Settings.Default.dbconnectionstring = FilePath;
                    db.Users.AddRange(Users.Select(x => x.User).Where(x => !db.Users.ToList().Exists(y => y.Login == x.Login)));
                    db.SaveChanges();
                    InvokeResponseEvent(ResponseType.Good, "База данных успешно создана");
                });
                InvokeLoadingStateEvent(false);
                InvokePageEnabledEvent(true);
                InvokeResponseEvent(ResponseType.Neutral, "");
            }
            InvokeCompleteStateEvent("next");
        }

        private void CancelBody()
        {
            InvokeCompleteStateEvent("cancel");
        }

        private void ClearBody()
        {
            Login = "";
            IsAdmin = false;
            Surname = "";
            FirstName = "";
            SecondName = "";
        }

        private void DeleteAllBody()
        {
            Users.Clear();
            DeleteAllEnabled = false;
        }

        private void DeleteUserBody()
        {
            Users.Remove(Users.First(x => x.User.Login == CurrentUser.User.Login));
        }

        private void ChangeUserBody()
        {
            DeleteAllEnabled = false;
            DeleteUserEnabled = false;
            Login = CurrentUser.User.Login;
            IsAdmin = CurrentUser.IsAdmin;
            Surname = CurrentUser.User.Surname;
            FirstName = CurrentUser.User.FirstName;
            SecondName = CurrentUser.User.SecondName;
            AddSaveUserBtnTitle = "Сохранить";
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

        private void AddFromExcelBody()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = "shell:MyComputerFolder",
                DefaultExt = ".xlsx",
                Title = "Выберите файл Excel",
                Filter = "Файл Excel|*.xlsx"
            };
            if ((bool)openFileDialog.ShowDialog())
            {
                object[,] arr;
                try
                {
                    using (var fs = new FileStream(openFileDialog.FileName, FileMode.Open))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        ExcelPackage excel = new ExcelPackage(fs);
                        excel.Load(fs);

                        try
                        {
                            arr = (object[,])excel.Workbook.Worksheets[0].Cells.Value;
                        }
                        catch (InvalidOperationException)
                        {
                            InvokeResponseEvent(ResponseType.Bad, "Невозможно открыть файл, т.к. он поврежден");
                            return;
                        }
                        excel.Dispose();
                    }
                }
                catch (IOException)
                {
                    InvokeResponseEvent(ResponseType.Bad, "Невозможно открыть файл, т.к. он занят другим процессом");
                    return;
                }

                if (arr is null)
                    InvokeResponseEvent(ResponseType.Bad, "Файл пуст");
                else if (!(arr.GetLength(1) == 4 || arr.GetLength(1) == 5))
                    InvokeResponseEvent(ResponseType.Bad, "Некорректный шаблон файла");
                else
                {
                    for (int i = 0; i < arr.GetLength(0); i++)
                    {
                        var newuser = new UserDTO()
                        {
                            IsAdmin = arr[i, 1] is null ? false : arr[i, 1].ToString() == "+",
                            User = new User()
                            {
                                Login = arr[i, 0]?.ToString(),
                                Surname = arr[i, 2]?.ToString(),
                                FirstName = arr[i, 3]?.ToString(),
                                SecondName = arr[i, 4]?.ToString(),
                            }
                        };
                        if (!Users.ToList().Exists(x => x.User.Login == newuser.User.Login))
                            Users.Add(newuser);
                    }
                    DeleteAllEnabled = true;
                    InvokeResponseEvent(ResponseType.Good, "Пользователи успешно добавлены из файла");
                }
            }
        }

        private void AddUserBody()
        {
            if (AddSaveUserBtnTitle == "Добавить")
            {
                if (Users.ToList().Exists(x => x.User.Login == Login))
                    InvokeResponseEvent(ResponseType.Bad, "Пользователь с таким логином уже добавлен");
                else
                {
                    Users.Add(new UserDTO() { User = new User(0, Login, null, Surname, FirstName, SecondName), IsAdmin = IsAdmin });
                    DeleteAllEnabled = true;
                    InvokeResponseEvent(ResponseType.Good, "Пользователь успешно добавлен");
                }
            }
            else
            {
                if (CurrentUser.User.Login != Login && Users.ToList().Exists(x => x.User.Login == Login))
                    InvokeResponseEvent(ResponseType.Bad, "Пользователь с таким логином уже добавлен");
                else
                {
                    var index = Users.IndexOf(CurrentUser);
                    Users[index] = new UserDTO() { User = new User(0, Login, null, Surname, FirstName, SecondName), IsAdmin = IsAdmin };
                    AddSaveUserBtnTitle = "Добавить";
                    InvokeResponseEvent(ResponseType.Good, "Пользователь успешно отредактирован");
                    Clear.Execute();
                    DeleteUserEnabled = true;
                    DeleteAllEnabled = true;
                }
            }
        }
    }
}