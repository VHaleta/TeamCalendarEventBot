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
using Telegram.Bot.Types.ReplyMarkups;

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
            string result = $"{MessageConst.EventsOn} {date.ToString("dd.MM.yyyy")}:\n\n";
            var foundEvents = _allGeneralEvents.Where(x => x.Date == date && x.IsActive == true && x.Type != CalendarEventType.Birthday);
            foreach (var item in foundEvents)
            {
                result += $"● {item.Text}\n";
            }
            var birthdays = _allGeneralEvents.Where(x => x.Date.Day == date.Day && x.Date.Month == date.Month && x.Type == CalendarEventType.Birthday && x.IsActive == true);
            foreach (var item in birthdays)
            {
                result += $"● {item.Text}\n";
            }

            if (!foundEvents.Any() && !birthdays.Any()) result += MessageConst.NoEvents;
            await botClient.SendTextMessageAsync(user.ChatId, result);
        }

        public static async Task ShowCalendarEventsByWeekAsync(ITelegramBotClient botClient, DateTime date, UserBot user)
        {
            string result = "";
            for (int i = date.Day; i <= date.Day + 7; i++)
            {
                DateTime tempDate = new DateTime(date.Year, date.Month, i);
                var foundEvents = _allGeneralEvents.Where(x => x.Date == tempDate && x.IsActive == true && x.Type != CalendarEventType.Birthday);
                var birthdays = _allGeneralEvents.Where(x => x.Date.Day == tempDate.Day && x.Date.Month == tempDate.Month && x.Type == CalendarEventType.Birthday && x.IsActive == true);
                if (foundEvents.Any() || birthdays.Any()) result += $"\nНа {DateConverter.EngToRusDay(tempDate.DayOfWeek.ToString())}:\n";
                foreach (var item in foundEvents)
                {
                    result += $"● {item.Text}\n";
                }
                foreach (var item in birthdays)
                {
                    result += $"● {item.Text}\n";
                }
            }
            if (result == "") result = MessageConst.NoEvents;
            await botClient.SendTextMessageAsync(user.ChatId, result);

        }

        public static int CountCalendarEventsByDate(DateTime date)
        {
            var foundEvents = _allGeneralEvents.Where(x => x.Date == date && x.IsActive == true && x.Type != CalendarEventType.Birthday);
            var birthdays = _allGeneralEvents.Where(x => x.Date.Day == date.Day && x.Date.Month == date.Month && x.Type == CalendarEventType.Birthday && x.IsActive == true);
            return foundEvents.Count() + birthdays.Count();
        }

        public static async Task EditCalendarEventsByDateAsync(ITelegramBotClient botClient, DateTime date, UserBot user)
        {
            var foundEvents = _allGeneralEvents.Where(x => x.Date == date && x.IsActive == true && x.Type != CalendarEventType.Birthday);
            var birthdays = _allGeneralEvents.Where(x => x.Date.Day == date.Day && x.Date.Month == date.Month && x.Type == CalendarEventType.Birthday && x.IsActive == true);
            List<InlineKeyboardButton> keyboardButtons;
            foreach (var item in foundEvents)
            {
                keyboardButtons = new List<InlineKeyboardButton> { new InlineKeyboardButton(MessageConst.Delete) { CallbackData = $"{CallbackConst.DeleteEvent} {item.Id}" }};
                await botClient.SendTextMessageAsync(user.ChatId, item.Text, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
            }
            foreach (var item in birthdays)
            {
                keyboardButtons = new List<InlineKeyboardButton> { new InlineKeyboardButton(MessageConst.Delete) { CallbackData = $"{CallbackConst.DeleteEvent} {item.Id}" } };
                await botClient.SendTextMessageAsync(user.ChatId, item.Text, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
            }
        }

        public static void AddGeneralEventAsync(CalendarEvent calendarEvent)
        {
            _allGeneralEvents.Add(calendarEvent);
            _dataProvider.AddGeneralEvent(calendarEvent);
        }

        public static void DeleteGeneralEvent(Guid ID)
        {
            var foundEvent = _allGeneralEvents.FirstOrDefault(x => x.Id == ID);
            if (foundEvent == null)
                return;
            _allGeneralEvents.Remove(foundEvent);
            _dataProvider.DeleteGeneralEvent(foundEvent);
        }

        public static void DeleteGeneralEvent(CalendarEvent calendarEvent)
        {
            _allGeneralEvents.Remove(calendarEvent);
            _dataProvider.DeleteGeneralEvent(calendarEvent);
        }

        public static CalendarEvent FindEvent(Guid eventId)
        {
            var foundEvent = _allGeneralEvents.FirstOrDefault(x => x.Id == eventId);
            return foundEvent;
        }

        public static void EditEvent(CalendarEvent calendarEvent)
        {
            var foundEvent = _allGeneralEvents.FirstOrDefault(x => x.Id == calendarEvent.Id);
            if (foundEvent == null)
                return;
            _allGeneralEvents.Remove(foundEvent);
            _dataProvider.DeleteGeneralEvent(foundEvent);
            _allGeneralEvents.Add(calendarEvent);
            _dataProvider.AddGeneralEvent(calendarEvent);
        }

        public static List<CalendarEvent> GetCalendarEventsForNotification()
        {
            var calendarEvents = _allGeneralEvents.Where(x => x.IsActive == true && x.Notifications > 0 && x.Date >= DateTime.Today);
            return calendarEvents.ToList();
        }
    }
}
