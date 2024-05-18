using AutoMapper;
using TeamCalendarEventBot.DAL.DataModels;
using TeamCalendarEventBot.DAL.Persistence;
using TeamCalendarEventBot.Domain.Repositories;
using TeamCalendarEventBot.Models.Models;

namespace TeamCalendarEventBot.DAL.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IEventDao _eventDao;
        private readonly IMapper _mapper;

        public EventRepository(IEventDao iventDao, IMapper mapper)
        {
            _eventDao = iventDao;
            _mapper = mapper;
        }

        public void AddGeneralEvent(CalendarEvent calendarEvent)
        {
            var calendarEventData = _mapper.Map<CalendarEventData>(calendarEvent);
            _eventDao.AddGeneralEvent(calendarEventData);
        }

        public void DeleteGeneralEvent(Guid eventId)
        {
            _eventDao.DeleteGeneralEvent(eventId);
        }

        public IEnumerable<CalendarEvent> GetGeneralEvents()
        {
            var eventsData = _eventDao.GetGeneralEvents();
            var events = _mapper.Map<List<CalendarEvent>>(eventsData);
            return events;
        }
    }
}
