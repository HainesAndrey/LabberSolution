using LabberLib.DataBaseContext.Entities;

namespace LabberClient.Workspace.AdminTab.JournalsCreater
{
    public class JournalDTO
    {
        public uint Id { get; set; }
        public string Group { get; set; }
        public string SubGroup { get; set; }
        public string Subject { get; set; }
        public string Teacher { get; set; }

        public JournalDTO(uint id, string group, string subGroup, string subject, string teacher)
        {
            Id = id;
            Group = group;
            SubGroup = subGroup;
            Subject = subject;
            Teacher = teacher;
        }

        public JournalDTO(Journal journal)
        {
            Id = journal.Id;
            Group = journal.Group.Title;
            SubGroup = journal.SubGroup;
            Subject = journal.Subject.ShortTitle;
            Teacher = ShortFullName(journal.User);
        }

        private string ShortFullName(User user)
        {
            return $"{user.Surname} {user.FirstName[0]}.{user.SecondName[0]}.";
        }
    }
}
