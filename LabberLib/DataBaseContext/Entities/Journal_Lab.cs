namespace LabberLib.DataBaseContext.Entities
{
    public class Journal_Lab
    {
        public uint Id { get; set; }
        public string Date { get; set; }
        public bool HasTheory { get; set; }

        public uint JournalId { get; set; }
        public Journal Journal { get; set; }

        public uint LabId { get; set; }
        public Lab Lab { get; set; }

        public Journal_Lab() { }

        public Journal_Lab(uint journalId, uint labId, string date, bool hasTheory)
        {
            JournalId = journalId;
            LabId = labId;
            Date = date;
            HasTheory = hasTheory;
        }
    }
}