using System;
using System.Collections.Generic;
using System.Text;
using TeamCalendarEventBot.Constants;

namespace TeamCalendarEventBot.Models
{
    public class CalendarEvent
    {
        public CalendarEvent() { Id = Guid.NewGuid(); IsActive = false; }
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public CalendarEventType Type { get; set; }
        public int Notifications { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }
    }
}
