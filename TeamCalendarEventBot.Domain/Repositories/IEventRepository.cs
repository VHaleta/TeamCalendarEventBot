using TeamCalendarEventBot.Models.Models;

namespace TeamCalendarEventBot.Domain.Repositories
{
    public interface IEventRepository
    {
        IEnumerable<CalendarEvent> GetGeneralEvents();

        void AddGeneralEvent(CalendarEvent calendarEvent);

        void DeleteGeneralEvent(Guid eventId);
    }
}
