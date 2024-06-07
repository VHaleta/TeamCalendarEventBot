namespace TeamCalendarEventBot.DAL.DataModels
{
    public class UserBotData
    {
        public long ChatId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Permissions { get; set; }
        public bool Active { get; set; }
        public bool GetNotification { get; set; }
        public int Auth { get; set; }
        public int UserStatus { get; set; }
        public int MenuStage { get; set; }
        public DateTime TempDate { get; set; }
    }
}
