using System;
using System.Collections.Generic;
using System.Text;
using TeamCalendarEventBot.Constant;
using TeamCalendarEventBot.Constants;

namespace TeamCalendarEventBot.Models
{
    public class UserBot
    {
        public long ChatId { get; set; }
        public string UserId { get; set; }
        public int Permissions { get; set; }
        public bool Active { get; set; }
        public AuthenticationState Auth { get; set; }
    }
}
