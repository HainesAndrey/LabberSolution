using System;

namespace LabberLib.DataBaseContext.Entities
{
    public class User
    {
        public uint Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; } = new Random().Next(100000, 999999).ToString();
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }

        public uint RoleId { get; set; } = 2;
        public Role Role { get; set; }

        public User(uint roleId, string login, string password = null, string surname = null, string firstName = null, string secondName = null)
        {
            RoleId = roleId;
            Login = login;
            if (password != null)
                Password = password;
            Surname = surname;
            FirstName = firstName;
            SecondName = secondName;
        }

        public User() { }

        public User UserWithoutPassword()
        {
            Password = null;
            return this;
        }
    }
}