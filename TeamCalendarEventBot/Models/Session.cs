using System;
using System.Collections.Generic;
using System.Text;
using TeamCalendarEventBot.Constant;

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
        public Person User { get; }
    }
}
