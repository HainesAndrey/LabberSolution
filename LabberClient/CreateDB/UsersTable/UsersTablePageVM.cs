using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using Microsoft.Win32;
using MvvmCross.Commands;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace LabberClient.CreateDB.UsersTable
{
    public class UsersTablePageVM : LabberVMBase
    {
        private string addSaveUserBtnTitle = "Добавить";
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
        private bool canChangeIsAdmin = true;
        private List<UserDTO> users = new List<UserDTO>();

        public string AddSaveUserBtnTitle { get => addSaveUserBtnTitle; set { addSaveUserBtnTitle = value; RaisePropertyChanged("AddSaveUserBtnTitle"); } }
        public bool AddUserEnabled { get => addUserEnabled; set { addUserEnabled = value; RaisePropertyChanged("AddUserEnabled"); } }
        public bool DeleteUserEnabled { get => deleteUserEnabled; set { deleteUserEnabled = value; RaisePropertyChanged("DeleteUserEnabled"); } }
        public bool DeleteAllEnabled { get => deleteAllEnabled; set { deleteAllEnabled = value; RaisePropertyChanged("DeleteAllEnabled"); } }
        public bool ClearEnabled { get => clearEnabled; set { clearEnabled = value; RaisePropertyChanged("ClearEnabled"); } }
        public UserDTO CurrentUser { get => currentUser; set { currentUser = value; RaisePropertyChanged("CurrentUser"); } }
        public bool IsAdmin { get => isAdmin; set { isAdmin = value; ClearEnabled = !IsAllFieldAreEmpty(); RaisePropertyChanged("IsAdmin"); } }
        public bool CanChangeIsAdmin { get => canChangeIsAdmin; set { canChangeIsAdmin = value; RaisePropertyChanged("CanChangeIsAdmin"); } }
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

        bool CheckRequiredFields() => Login != "" && Surname != "" && FirstName != "";
        bool IsAllFieldAreEmpty() => Login == "" && Surname == "" && FirstName == "" && SecondName == "" && !IsAdmin;

        public List<UserDTO> Users { get => users; set { users = value; RaisePropertyChanged("Users"); } }
        public MvxCommand AddUser { get; set; }
        public MvxCommand ChangeUser { get; set; }
        public MvxCommand DeleteUser { get; set; }
        public MvxCommand Clear { get; set; }
        public MvxCommand DeleteAll { get; set; }
        public MvxCommand AddFromExcel { get; set; }

        public UsersTablePageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            AddUser = new MvxCommand(AddUserBody);
            ChangeUser = new MvxCommand(ChangeUserBody);
            AddFromExcel = new MvxCommand(AddFromExcelBody);
            DeleteUser = new MvxCommand(DeleteUserBody);
            DeleteAll = new MvxCommand(DeleteAllBody);
            Clear = new MvxCommand(ClearBody);
        }

        public override void LoadData()
        {
            if (DBWorker.FilePath == "")
                return;

            //InvokePageEnabledEvent(false);

            Refresh();
            //InvokePageEnabledEvent(true);
        }

        private async void Refresh()
        {
            InvokeLoadingStateEvent(true);
            if (DBWorker.FilePath != "")
                await Task.Run(() =>
                {
                    using (db = new DBWorker())
                    {
                        Users = db.Users.ToList().Select(x => new UserDTO(x)).ToList();
                    }
                });
            if (Users.Count > 1)
                DeleteAllEnabled = true;
            var view = (CollectionView)CollectionViewSource.GetDefaultView(Users);
            view.SortDescriptions.Add(new SortDescription("IsAdmin", ListSortDirection.Descending));
            view.SortDescriptions.Add(new SortDescription("User.Surname", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("User.FirstName", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("User.SecondName", ListSortDirection.Ascending));
            InvokeLoadingStateEvent(false);
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
            using (db = new DBWorker())
            {
                db.Users.RemoveRange(Users.Where(x => x.User.Id != 1).Select(x => x.User));
            }
            InvokeResponseEvent(ResponseType.Good, "Пользователи успешно удалены");
            Refresh();
            DeleteAllEnabled = false;
        }

        private void DeleteUserBody()
        {
            if (CurrentUser.User.Id == 1 || CurrentUser.User.Id == DBWorker.UserId)
                InvokeResponseEvent(ResponseType.Bad, "Данного пользователя удалить невозможно");
            else
            {
                using (db = new DBWorker())
                {
                    db.Users.Remove(Users.First(x => x.User.Login == CurrentUser.User.Login).User);
                }
                InvokeResponseEvent(ResponseType.Good, "Пользователь успешно удален");
                Refresh();
            }
        }

        private void ChangeUserBody()
        {
            if (CurrentUser.User.Id == 1)
            {
                InvokeResponseEvent(ResponseType.Bad, "Информацию о данном пользователе редактировать невозможно");
                return;
            }
            DeleteAllEnabled = false;
            DeleteUserEnabled = false;
            Login = CurrentUser.User.Login;
            IsAdmin = CurrentUser.IsAdmin;
            Surname = CurrentUser.User.Surname;
            FirstName = CurrentUser.User.FirstName;
            SecondName = CurrentUser.User.SecondName;
            AddSaveUserBtnTitle = "Сохранить";
            CanChangeIsAdmin = DBWorker.UserId != CurrentUser.User.Id;

        }

        private async void AddFromExcelBody()
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
                object[,] arr = null;
                InvokeLoadingStateEvent(true);
                InvokePageEnabledEvent(false);
                InvokeResponseEvent(ResponseType.Neutral, "Подождите...");
                try
                {
                    await Task.Run(() =>
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
                                InvokePageEnabledEvent(true);
                                InvokeLoadingStateEvent(false);
                                return;
                            }
                            excel.Dispose();
                        }
                    });
                }
                catch (IOException)
                {
                    InvokeResponseEvent(ResponseType.Bad, "Невозможно открыть файл, т.к. он занят другим процессом");
                    InvokePageEnabledEvent(true);
                    InvokeLoadingStateEvent(false);
                    return;
                }

                if (arr is null)
                    InvokeResponseEvent(ResponseType.Bad, "Файл пуст");
                else if (!(arr.GetLength(1) == 4 || arr.GetLength(1) == 5))
                    InvokeResponseEvent(ResponseType.Bad, "Некорректный шаблон файла");
                else
                {
                    try
                    {
                        await Task.Run(() =>
                        {
                            List<User> users = new List<User>();
                            for (int i = 0; i < arr.GetLength(0); i++)
                            {
                                var newuser = new UserDTO()
                                {
                                    IsAdmin = arr[i, 1] is null ? false : arr[i, 1].ToString() == "админ",
                                    User = new User()
                                    {
                                        RoleId = (uint)((arr[i, 1] is null ? false : arr[i, 1].ToString() == "админ") ? 1 : 2),
                                        Login = arr[i, 0]?.ToString(),
                                        Surname = arr[i, 2]?.ToString(),
                                        FirstName = arr[i, 3]?.ToString(),
                                        SecondName = arr[i, 4]?.ToString(),
                                    }
                                };
                                if (!users.ToList().Exists(x => x.Login == newuser.User.Login))
                                {
                                    users.Add(newuser.User);
                                    using (db = new DBWorker(true))
                                    {
                                        db.Users.Add(newuser.User);
                                    }
                                }
                            }


                        });
                        Refresh();
                    }
                    catch (FileNotFoundException)
                    {
                        InvokeResponseEvent(ResponseType.Bad, "Укажите корректный путь для новой базы данных");
                        InvokePageEnabledEvent(true);
                        InvokeLoadingStateEvent(false);
                        return;
                    }

                    DeleteAllEnabled = true;
                    InvokeResponseEvent(ResponseType.Good, "Пользователи успешно загружены из файла");
                }
                InvokeLoadingStateEvent(false);
                InvokePageEnabledEvent(true);
            }
        }

        private async void AddUserBody()
        {
            if (AddSaveUserBtnTitle == "Добавить")
            {
                if (Login.Replace(" ", "") == "" || Surname.Replace(" ", "") == "" || FirstName.Replace(" ", "") == "")
                    InvokeResponseEvent(ResponseType.Bad, "Информация о пользователе некорректна");
                else if (Users.ToList().Exists(x => x.User.Login == Login))
                    InvokeResponseEvent(ResponseType.Bad, "Пользователь с таким логином уже добавлен");
                else if (Users.Select(x => x.User).ToList().Exists(x => x.Surname == Surname && x.FirstName == FirstName && x.SecondName == SecondName))
                    InvokeResponseEvent(ResponseType.Bad, "Пользователь с такими ФИО уже добавлен");
                else
                {
                    InvokeLoadingStateEvent(true);
                    InvokeResponseEvent(ResponseType.Neutral, "Подождите");
                    try
                    {
                        await Task.Run(() =>
                        {
                            using (db = new DBWorker(true))
                            {
                                db.Users.Add(new User((uint)(IsAdmin ? 1 : 2), Login, Surname, FirstName, SecondName));
                            }
                        });
                        Refresh();
                    }
                    catch (FileNotFoundException)
                    {
                        InvokeResponseEvent(ResponseType.Bad, "Укажите корректный путь для новой базы данных");
                        InvokeLoadingStateEvent(false);
                        return;
                    }

                    DeleteAllEnabled = true;
                    InvokeResponseEvent(ResponseType.Good, "Пользователь успешно добавлен");
                    InvokeLoadingStateEvent(false);
                }
            }
            else
            {
                if (Login.Replace(" ", "") == "" || Surname.Replace(" ", "") == "" || FirstName.Replace(" ", "") == "")
                    InvokeResponseEvent(ResponseType.Bad, "Информация о пользователе некорректна");
                else if (CurrentUser.User.Login != Login && Users.ToList().Exists(x => x.User.Login == Login))
                    InvokeResponseEvent(ResponseType.Bad, "Пользователь с таким логином уже добавлен");
                else
                {
                    using (db = new DBWorker())
                    {
                        var user = db.Users.First(x => x.Id == CurrentUser.User.Id);
                        user.Login = Login;
                        user.Surname = Surname;
                        user.FirstName = FirstName;
                        user.SecondName = SecondName;
                        //user.Password = 
                        user.RoleId = (uint)(IsAdmin ? 1 : 2);
                    }
                    Refresh();
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
