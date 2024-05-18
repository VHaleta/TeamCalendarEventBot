using System.Collections.Generic;
using TeamCalendarEventBot.Models;

namespace TeamCalendarEventBot.DataStorage
{
    public interface IEventDataProvider
    {
        List<CalendarEvent> GetGeneralEvents();
        void AddGeneralEvent(CalendarEvent calendarEvent);
        void DeleteGeneralEvent(CalendarEvent calendarEvent);
    }
}