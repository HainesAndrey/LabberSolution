namespace LabberLib.DataBaseContext.Entities
{
    public class Mark
    {
        public uint Id { get; set; }
        public string PracticeState { get; set; }
        public string TheoryState { get; set; }
        public string Date { get; set; }
        public bool WasntOnLab { get; set; }

        public uint StudentId { get; set; }
        public Student Student { get; set; }

        public uint Journal_LabId { get; set; }
        public Journal_Lab Journal_Lab { get; set; }

        public Mark() { }

        public Mark(uint studentId, uint journal_LabId, string practiceState, string theoryState, string date, bool wasntOnLab = true)
        {
            StudentId = studentId;
            Journal_LabId = journal_LabId;
            PracticeState = practiceState;
            TheoryState = theoryState;
            Date = date;
            WasntOnLab = wasntOnLab;
        }
    }
}