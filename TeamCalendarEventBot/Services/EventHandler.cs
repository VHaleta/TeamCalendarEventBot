using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCalendarEventBot.DataStorage;
using TeamCalendarEventBot.DataStorage.DataJsonFile;
using TeamCalendarEventBot.Helpers;
using TeamCalendarEventBot.Models;
using Telegram.Bot;

namespace TeamCalendarEventBot.Services
{
    public static class EventHandler
    {
        private static readonly IEventDataProvider _dataProvider;
//        private static readonly object _locker = new();
        private static readonly List<CalendarEvent> _allGeneralEvents;

        static EventHandler()
        {
            _dataProvider = new JsonFileDataClient().EventDataProvider;
            _allGeneralEvents = _dataProvider.GetGeneralEvents();
            _allGeneralEvents.Add(new CalendarEvent() { Id = 1, Date = new DateTime(2022, 3, 12), Header = "Событие 1", Text = "qqqqqqqq q qqqqqq q qqqq  qq q qq qqq q qq q q q qqq qq" });
            _allGeneralEvents.Add(new CalendarEvent() { Id = 2, Date = new DateTime(2022, 3, 12), Header = "Событие 2", Text = "qqqqqqqq q qqqqqq q qqqq  qq q qq qqq q qq q q q qqq qq" });
            _allGeneralEvents.Add(new CalendarEvent() { Id = 3, Date = new DateTime(2022, 3, 12), Header = "Событие 3", Text = "qqqqqqqq q qqqqqq q qqqq  qq q qq qqq q qq q q q qqq qq" });
            _allGeneralEvents.Add(new CalendarEvent() { Id = 4, Date = new DateTime(2022, 4, 12), Header = "Событие 4", Text = "qqqqqqqq q qqqqqq q qqqq  qq q qq qqq q qq q q q qqq qq" });
        }

        public static async Task ShowCalendarEventsByDateAsync(ITelegramBotClient botClient, DateTime date, UserBot user)
        {
            string result = $"События на {ZeroAdder.AddZero(date.Day)}.{ZeroAdder.AddZero(date.Month)}.{date.Year}\n\n";
            int count = 0;
            foreach (var item in _allGeneralEvents)
            {
                if(item.Date == date)
                {
                    result += $"{item.Header}\n{item.Text}\n\n";
                    count++;
                }
            }
            if (count == 0) result += "Событий нет";
            await botClient.SendTextMessageAsync(user.ChatId, result);
        }

        public static void AddGeneralEvent(CalendarEvent calendarEvent)
        {
            _allGeneralEvents.Add(calendarEvent);
            _dataProvider.AddGeneralEvent(calendarEvent);
        }

        public static void DeleteGeneralEvent(CalendarEvent calendarEvent)
        {
            _allGeneralEvents.Remove(calendarEvent);
            _dataProvider.DeleteGeneralEvent(calendarEvent);
        }
    }
}
