using System;
using TeamCalendarEventBot.Constants;

namespace TeamCalendarEventBot.Models
{
    public class Session
    {
        public Session()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
        public MenuStage MenuStage { get; set; }
        public UserBot User { get; }
    }
}
