using LabberLib.DataBaseContext.Entities;

namespace LabberClient.CreateDB
{
    public class UserDTO
    {
        public User User { get; set; } = new User();
        public bool IsAdmin { get; set; }

        public UserDTO() { }

        public bool IsAnyPropertyEmpty()
        {
            return
                User.Login is null || User.Login == "" ||
                User.Surname is null || User.Surname == "" ||
                User.FirstName is null || User.FirstName == "" ||
                User.SecondName is null || User.SecondName == "";
        }
    }
}