using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCalendarEventBot.Models.Models;
using Telegram.Bot;

namespace TeamCalendarEventBot.Domain.Processor.Services
{
    public interface IEventService
    {
        public Task ShowCalendarEventsByDateAsync(ITelegramBotClient botClient, DateTime date, UserBot user);
        public Task ShowCalendarEventsByWeekAsync(ITelegramBotClient botClient, DateTime date, UserBot user);
        public int CountCalendarEventsByDate(DateTime date);
        public Task EditCalendarEventsByDateAsync(ITelegramBotClient botClient, DateTime date, UserBot user);
        public void AddGeneralEventAsync(CalendarEvent calendarEvent);
        public void DeleteGeneralEvent(Guid ID);
        public void DeleteGeneralEvent(CalendarEvent calendarEvent);
        public CalendarEvent FindEvent(Guid eventId);
        public void EditEvent(CalendarEvent calendarEvent);
        public List<CalendarEvent> GetCalendarEventsForNotification();
    }
}
