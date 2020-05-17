namespace LabberLib.DataBaseContext.Entities
{
    public class Student
    {
        public uint Id { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string SubGroup { get; set; }

        public uint GroupId { get; set; }
        public Group Group { get; set; }

        public Student(uint groupId, string surname, string firstName, string secondName, string subGroup)
        {
            GroupId = groupId;
            Surname = surname;
            FirstName = firstName;
            SecondName = secondName;
            SubGroup = subGroup;
        }
    }
}