using Newtonsoft.Json;
using TeamCalendarEventBot.DAL.DataModels;
using TeamCalendarEventBot.DAL.Persistence.FileProvider;

namespace TeamCalendarEventBot.DAL.Persistence
{
    public class EventDao : IEventDao
    {
        private const string _fileName = "general_events.dat";
        private static object _locker = new object();
        private readonly IFileProvider _fileProvider;

        public EventDao(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public void AddGeneralEvent(CalendarEventData calendarEvent)
        {
            lock (_locker)
            {
                var calendarEvents = GetGeneralEvents().ToList();
                calendarEvents.Add(calendarEvent);

                calendarEvents = calendarEvents.OrderBy(x => x.Date).ToList();
                var newDataContents = JsonConvert.SerializeObject(calendarEvents);
                _fileProvider.WriteFile(_fileName, newDataContents);
            }
        }

        public void DeleteGeneralEvent(Guid eventId)
        {
            lock (_locker)
            {
                var calendarEvents = GetGeneralEvents().ToList();
                for (int i = 0; i < calendarEvents.Count; i++)
                {
                    if (calendarEvents[i].Id == eventId)
                    {
                        calendarEvents.RemoveAt(i);
                    }
                }
                var newDataContents = JsonConvert.SerializeObject(calendarEvents);
                _fileProvider.WriteFile(_fileName, newDataContents);
            }
        }

        public IEnumerable<CalendarEventData> GetGeneralEvents()
        {
            lock (_locker)
            {
                var dataContents = _fileProvider.ReadFile(_fileName);
                if (string.IsNullOrWhiteSpace(dataContents))
                {
                    return new List<CalendarEventData>();
                }

                return JsonConvert.DeserializeObject<List<CalendarEventData>>(dataContents) ?? new List<CalendarEventData>();

            }
        }
    }
}
