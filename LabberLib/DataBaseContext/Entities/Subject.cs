namespace LabberLib.DataBaseContext.Entities
{
    public class Subject
    {
        public uint Id { get; set; }
        public string ShortTitle { get; set; }
        public string LongTitle { get; set; }

        public Subject(string shortTitle, string longTitle)
        {
            ShortTitle = shortTitle;
            LongTitle = longTitle;
        }
    }
}