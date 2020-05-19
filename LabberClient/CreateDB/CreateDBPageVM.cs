using GalaSoft.MvvmLight.CommandWpf;
using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;
using System.Collections.ObjectModel;
using System.Linq;

namespace LabberClient.CreateDB
{
    public class CreateDBPageVM : LabberVMBase
    {
        private bool addUserEnabled = false;
        private bool deleteAllEnabled = false;
        private bool clearEnabled = false;
        private string login = "";
        private bool isAdmin = false;
        private string surname = "";
        private string firstName = "";
        private string secondName = "";
        private UserDTO currentUser;

        public ObservableCollection<UserDTO> Users { get; set; }
        public RelayCommand AddUser { get; set; }
        public RelayCommand ChangeUser { get; set; }
        public RelayCommand DeleteUser { get; set; }
        public RelayCommand Clear { get; set; }
        public RelayCommand DeleteAll { get; set; }
        public RelayCommand AddFromExcel { get; set; }
        public RelayCommand Cancel { get; set; }
        public RelayCommand Next { get; set; }

        public bool AddUserEnabled { get => addUserEnabled; set { addUserEnabled = value; OnPropertyChanged("AddUserEnabled"); } }
        public bool DeleteAllEnabled { get => deleteAllEnabled; set { deleteAllEnabled = value; OnPropertyChanged("DeleteAllEnabled"); } }
        public bool ClearEnabled { get => clearEnabled; set { clearEnabled = value; OnPropertyChanged("ClearEnabled"); } }

        public UserDTO CurrentUser { get => currentUser; set { currentUser = value; OnPropertyChanged("CurrentUser"); } }

        public string Login
        {
            get => login;
            set
            {
                login = value;
                AddUserEnabled = CheckFields();
                ClearEnabled = !IsAllFieldAreEmpty();
                OnPropertyChanged("Login");
            }
        }
        public string Surname
        {
            get => surname;
            set
            {
                surname = value;
                AddUserEnabled = CheckFields();
                ClearEnabled = !IsAllFieldAreEmpty();
                OnPropertyChanged("Surname");
            }
        }
        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                AddUserEnabled = CheckFields();
                ClearEnabled = !IsAllFieldAreEmpty();
                OnPropertyChanged("FirstName");
            }
        }

        public bool IsAdmin { get => isAdmin; set { isAdmin = value; ClearEnabled = !IsAllFieldAreEmpty(); OnPropertyChanged("IsAdmin"); } }
        public string SecondName { get => secondName; set { secondName = value; ClearEnabled = !IsAllFieldAreEmpty(); OnPropertyChanged("SecondName"); } }


        bool CheckFields()
        {
            return Login != "" && Surname != "" && FirstName != "";
        }

        bool IsAllFieldAreEmpty()
        {
            return Login == "" && Surname == "" && FirstName == "" && SecondName == "" && !IsAdmin;
        }

        public CreateDBPageVM(uint userId, string dbconnectionstring, ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(userId, dbconnectionstring, ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            Users = new ObservableCollection<UserDTO>();

            AddUser = new RelayCommand(() =>
            {
                if (Users.ToList().Exists(x => x.User.Login == Login))
                    InvokeResponseEvent(ResponseType.Bad, "Пользователь с таким логином уже добавлен");
                else
                {
                    Users.Add(new UserDTO() { User = new User(0, Login, null, Surname, FirstName, SecondName), IsAdmin = IsAdmin });
                    DeleteAllEnabled = true;
                }
            });

            ChangeUser = new RelayCommand(() =>
            {
                Login = CurrentUser.User.Login;
                IsAdmin = CurrentUser.IsAdmin;
                Surname = CurrentUser.User.Surname;
                FirstName = CurrentUser.User.FirstName;
                SecondName = CurrentUser.User.SecondName;
            });

            DeleteAll = new RelayCommand(() =>
            {
                Users.Clear();
                DeleteAllEnabled = false;
            });

            Clear = new RelayCommand(() =>
            {
                Login = "";
                IsAdmin = false;
                Surname = "";
                FirstName = "";
                SecondName = "";
            });

            Cancel = new RelayCommand(() =>
            {
                InvokeCompleteStateEvent("cancel");
            });
        }
    }
}