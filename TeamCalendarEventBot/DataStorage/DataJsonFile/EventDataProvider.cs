using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCalendarEventBot.Models;

namespace TeamCalendarEventBot.DataStorage.DataJsonFile
{
    public class EventDataProvider : JsonFileDataProviderBase, IEventDataProvider
    {
        private const string _fileName = "general_events.dat";
        private static object _locker = new object();
        public EventDataProvider(IFileProvider fileProvider) : base(fileProvider)
        {
            
        }

        public void AddGeneralEvent(CalendarEvent calendarEvent)
        {
            lock (_locker)
            {
                var calendarEvents = GetGeneralEvents();
                calendarEvents.Add(calendarEvent);

                calendarEvents = calendarEvents.OrderBy(x => x.Date).ToList();
                var newDataContents = JsonConvert.SerializeObject(calendarEvents);
                FileProvider.WriteFile(_fileName, newDataContents);

            }
        }

        public void DeleteGeneralEvent(CalendarEvent calendarEvent)
        {
            lock (_locker)
            {
                var calendarEvents = GetGeneralEvents();
                for (int i = 0; i < calendarEvents.Count; i++)
                {
                    if(calendarEvents[i].Id == calendarEvent.Id)
                    {
                        calendarEvents.RemoveAt(i);
                    }
                }
                var newDataContents = JsonConvert.SerializeObject(calendarEvents);
                FileProvider.WriteFile(_fileName, newDataContents);
            }
        }

        public List<CalendarEvent> GetGeneralEvents()
        {
            lock (_locker)
            {
                var dataContents = FileProvider.ReadFile(_fileName);
                if (string.IsNullOrWhiteSpace(dataContents))
                {
                    return new List<CalendarEvent>();
                }

                return JsonConvert.DeserializeObject<List<CalendarEvent>>(dataContents);

            }
        }
    }
}
