namespace LabberLib.DataBaseContext.Entities
{
    public class Lab
    {
        public uint Id { get; set; }
        public double Number { get; set; }
        public string Title { get; set; }

        public Lab(double number, string title)
        {
            Number = number;
            Title = title;
        }
    }
}