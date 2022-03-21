using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamCalendarEventBot.Constants;
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
            string result = $"События на {date.ToString("dd.MM.yyyy")}\n\n";
            var foundEvents = _allGeneralEvents.Where(x => x.Date == date);
            foreach (var item in foundEvents)
            {
                result += $"{item.Header}\n{item.Text}\n\n";
            }

            if (!foundEvents.Any()) result += "Событий нет";
            await botClient.SendTextMessageAsync(user.ChatId, result);
        }

        public static async Task ShowCalendarEventsByWeekAsync(ITelegramBotClient botClient, DateTime date, UserBot user)
        {
            string result = "";
            int dayOfWeek = (int)date.DayOfWeek;
            for (int i = date.Day - dayOfWeek + 1; i <= date.Day + 7 - dayOfWeek; i++)
            {
                DateTime tempDate = new DateTime(date.Year, date.Month, i);
                var foundEvents = _allGeneralEvents.Where(x => x.Date == tempDate);
                if (foundEvents.Any()) result += $"На {DateConverter.EngToRusDay(tempDate.DayOfWeek.ToString())}\n\n";
                foreach (var item in foundEvents)
                {
                    result += $"{item.Header}\n{item.Text}\n\n";
                }
            }
            if (result == "") result = "Событий нет";
            await botClient.SendTextMessageAsync(user.ChatId, result);

        }

        public static async Task AddGeneralEvent(ITelegramBotClient botClient, UserBot user)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, "У вас недостаточно прав");
                return;
            }

//            await botClient.SendTextMessageAsync(user.ChatId, "Введите заглавие");
            //            _allGeneralEvents.Add(calendarEvent);
            //            _dataProvider.AddGeneralEvent(calendarEvent);
        }

        public static void DeleteGeneralEvent(CalendarEvent calendarEvent)
        {
            _allGeneralEvents.Remove(calendarEvent);
            _dataProvider.DeleteGeneralEvent(calendarEvent);
        }
    }
}
