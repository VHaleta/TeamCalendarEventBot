using System;
using TeamCalendarEventBot.Constants;

namespace TeamCalendarEventBot.Models
{
    public class UserBot
    {
        public long ChatId { get; set; }
        public string Username { get; set; }
        public int Permissions { get; set; }
        public bool Active { get; set; }
        public bool GetNotification { get; set; }
        public AuthenticationState Auth { get; set; }
        public UserStatus UserStatus { get; set; }
        public MenuStage MenuStage { get; set; }
        public DateTime TempDate { get; set; }
    }
}
