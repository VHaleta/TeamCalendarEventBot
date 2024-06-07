using TeamCalendarEventBot.DAL.DataModels;

namespace TeamCalendarEventBot.DAL.Persistence
{
    public interface IEventDao
    {
        public void AddGeneralEvent(CalendarEventData calendarEvent);

        public void DeleteGeneralEvent(Guid eventId);

        public IEnumerable<CalendarEventData> GetGeneralEvents();
    }
}
