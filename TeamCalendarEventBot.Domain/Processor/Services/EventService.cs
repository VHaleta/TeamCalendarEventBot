using TeamCalendarEventBot.Domain.Helpers;
using TeamCalendarEventBot.Domain.Repositories;
using TeamCalendarEventBot.Models.Constants;
using TeamCalendarEventBot.Models.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeamCalendarEventBot.Domain.Processor.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly List<CalendarEvent> _allGeneralEvents;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _allGeneralEvents = _eventRepository.GetGeneralEvents().ToList();
        }

        public async Task ShowCalendarEventsByDateAsync(ITelegramBotClient botClient, DateTime date, UserBot user)
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

        public async Task ShowCalendarEventsByWeekAsync(ITelegramBotClient botClient, DateTime date, UserBot user)
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

        public int CountCalendarEventsByDate(DateTime date)
        {
            var foundEvents = _allGeneralEvents.Where(x => x.Date == date && x.IsActive == true && x.Type != CalendarEventType.Birthday);
            var birthdays = _allGeneralEvents.Where(x => x.Date.Day == date.Day && x.Date.Month == date.Month && x.Type == CalendarEventType.Birthday && x.IsActive == true);
            return foundEvents.Count() + birthdays.Count();
        }

        public async Task EditCalendarEventsByDateAsync(ITelegramBotClient botClient, DateTime date, UserBot user)
        {
            var foundEvents = _allGeneralEvents.Where(x => x.Date == date && x.IsActive == true && x.Type != CalendarEventType.Birthday);
            var birthdays = _allGeneralEvents.Where(x => x.Date.Day == date.Day && x.Date.Month == date.Month && x.Type == CalendarEventType.Birthday && x.IsActive == true);
            List<InlineKeyboardButton> keyboardButtons;
            foreach (var item in foundEvents)
            {
                keyboardButtons = new List<InlineKeyboardButton> { new InlineKeyboardButton(MessageConst.Delete) { CallbackData = $"{CallbackConst.DeleteEvent} {item.Id}" } };
                await botClient.SendTextMessageAsync(user.ChatId, item.Text, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
            }
            foreach (var item in birthdays)
            {
                keyboardButtons = new List<InlineKeyboardButton> { new InlineKeyboardButton(MessageConst.Delete) { CallbackData = $"{CallbackConst.DeleteEvent} {item.Id}" } };
                await botClient.SendTextMessageAsync(user.ChatId, item.Text, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
            }
        }

        public void AddGeneralEventAsync(CalendarEvent calendarEvent)
        {
            _allGeneralEvents.Add(calendarEvent);
            _eventRepository.AddGeneralEvent(calendarEvent);
        }

        public void DeleteGeneralEvent(Guid ID)
        {
            var foundEvent = _allGeneralEvents.FirstOrDefault(x => x.Id == ID);
            if (foundEvent == null)
                return;
            _allGeneralEvents.Remove(foundEvent);
            _eventRepository.DeleteGeneralEvent(foundEvent.Id);
        }

        public void DeleteGeneralEvent(CalendarEvent calendarEvent)
        {
            _allGeneralEvents.Remove(calendarEvent);
            _eventRepository.DeleteGeneralEvent(calendarEvent.Id);
        }

        public CalendarEvent FindEvent(Guid eventId)
        {
            var foundEvent = _allGeneralEvents.FirstOrDefault(x => x.Id == eventId);
            return foundEvent;
        }

        public void EditEvent(CalendarEvent calendarEvent)
        {
            var foundEvent = _allGeneralEvents.FirstOrDefault(x => x.Id == calendarEvent.Id);
            if (foundEvent == null)
                return;
            _allGeneralEvents.Remove(foundEvent);
            _eventRepository.DeleteGeneralEvent(foundEvent.Id);
            _allGeneralEvents.Add(calendarEvent);
            _eventRepository.AddGeneralEvent(calendarEvent);
        }

        public List<CalendarEvent> GetCalendarEventsForNotification()
        {
            var calendarEvents = _allGeneralEvents.Where(x => x.IsActive == true && x.Notifications > 0 && x.Date >= DateTime.Today);
            return calendarEvents.ToList();
        }
    }
}
