namespace LabberLib.DataBaseContext.Entities
{
    public class Journal
    {
        public uint Id { get; set; }
        public string SubGroup { get; set; }

        public uint GroupId { get; set; }
        public Group Group { get; set; }

        public uint SubjectId { get; set; }
        public Subject Subject { get; set; }

        public uint UserId { get; set; }
        public User User { get; set; }

        public Journal() { }

        public Journal(uint groupId, uint subjectId, uint userId, string subGroup)
        {
            GroupId = groupId;
            SubjectId = subjectId;
            UserId = userId;
            SubGroup = subGroup;
        }
    }
}