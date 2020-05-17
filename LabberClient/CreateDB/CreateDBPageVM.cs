using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;
using System.Collections.ObjectModel;

namespace LabberClient.CreateDB
{
    public class CreateDBPageVM : LabberVMBase
    {
        public ObservableCollection<UserDTO> Users { get; set; }
        public CommandAsync AddUser { get; set; }
        public CommandAsync ChangeUser { get; set; }
        public CommandAsync Clear { get; set; }
        public CommandAsync DeleteAll { get; set; }
        public CommandAsync AddFromExcel { get; set; }
        public CommandAsync Cancel { get; set; }
        public CommandAsync Next { get; set; }

        public bool AddUserEnabled { get; set; } = false;
        public bool DeleteAllEnabled { get; set; } = false;
        public bool ClearEnabled { get; set; } = false;

        public UserDTO User { get; set; }

        public CreateDBPageVM(uint userId, string dbconnectionstring, ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(userId, dbconnectionstring, ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            Users = new ObservableCollection<UserDTO>();
            //Users.Add(new UserWithTeacher() { Login = "adminPOIT", IsAdmin = true });
            Users.Add(new UserDTO() { User = new User() { Login = "adminPOIT" }, IsAdmin = true });
        }
    }
}