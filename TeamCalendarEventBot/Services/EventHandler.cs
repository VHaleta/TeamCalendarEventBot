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
        }

        public static async Task ShowCalendarEventsByDateAsync(ITelegramBotClient botClient, DateTime date, UserBot user)
        {
            string result = $"События на {date.ToString("dd.MM.yyyy")}\n\n";
            var foundEvents = _allGeneralEvents.Where(x => x.Date == date);
            foreach (var item in foundEvents)
            {
                result += $"{item.Text}\n\n";
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
                    result += $"{item.Text}\n\n";
                }
            }
            if (result == "") result = "Событий нет";
            await botClient.SendTextMessageAsync(user.ChatId, result);

        }

        public static async Task AddGeneralEventAsync(ITelegramBotClient botClient, UserBot user, CalendarEvent calendarEvent)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, "У вас недостаточно прав");
                return;
            }

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
