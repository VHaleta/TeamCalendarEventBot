using System;
using System.Collections.Generic;
using System.Text;

namespace TeamCalendarEventBot.Models
{
    public class CalendarEvent
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string EventText { get; set; }
    }
}
