using LabberLib.DataBaseContext.Entities;

namespace LabberClient.CreateDB.UsersTable
{
    public class UserDTO
    {
        public User User { get; set; } = new User();
        public bool IsAdmin { get; set; }

        public UserDTO() { }

        public UserDTO(User user)
        {
            User = user;
            IsAdmin = user.RoleId == 1;
        }
    }
}