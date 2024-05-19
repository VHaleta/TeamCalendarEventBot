namespace TeamCalendarEventBot.DAL.DataModels
{
    public class CalendarEventData
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public int Notifications { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }
    }
}
