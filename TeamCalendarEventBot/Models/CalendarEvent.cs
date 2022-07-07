using System;
using System.Collections.Generic;
using System.Text;

namespace TeamCalendarEventBot.Models
{
    public class CalendarEvent
    {
        public CalendarEvent() { Id = Guid.NewGuid(); }
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}
