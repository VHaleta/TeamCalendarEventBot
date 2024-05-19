using Microsoft.Extensions.Logging;
using System.Timers;
using TeamCalendarEventBot.Models.Constants;
using TeamCalendarEventBot.Models.Models;
using TeamCalendarEventBot.Services;
using Telegram.Bot;

namespace TeamCalendarEventBot.Domain.Processor.Services
{
    public class NotificationService
    {
        private ITelegramBotClient bot;
        private readonly ILogger<NotificationService> _logger;
        private readonly EventService _eventService;
        private readonly UserService _userService;
        private System.Timers.Timer aTimer;
        public NotificationService(
            ILogger<NotificationService> logger,
            EventService eventService,
            UserService userService)
        {
            _logger = logger;
            _eventService = eventService;
            _userService = userService;

            aTimer = new System.Timers.Timer(600000);
            aTimer.Elapsed += CheckNotifications;
        }
        private async void CheckNotifications(Object source, ElapsedEventArgs e)
        {
            _logger.LogDebug("Notification check");
            List<CalendarEvent> calendarEvents = _eventService.GetCalendarEventsForNotification();
            string result = "", inDay = "", forOneDay = "", forTwoDays = "", forAWeek = "";
            bool doesInDay = false, doesForOneDay = false, doesForTwoDays = false, doesForAWeek = false;
            foreach (var calendarEvent in calendarEvents)
            {
                Notification notifications = (Notification)calendarEvent.Notifications;
                if ((((Notification)calendarEvent.Notifications & Notification.InDay) == Notification.InDay) && (calendarEvent.Date == DateTime.Today))
                {
                    inDay += $"● {calendarEvent.Text}\n";
                    notifications &= ~Notification.InDay;
                    doesInDay = true;
                    calendarEvent.Notifications = (int)notifications;
                    _eventService.EditEvent(calendarEvent);
                }
                if ((((Notification)calendarEvent.Notifications & Notification.ForOneDay) == Notification.ForOneDay) && (calendarEvent.Date == DateTime.Today.AddDays(+1)))
                {
                    forOneDay += $"● {calendarEvent.Text}\n";
                    notifications &= ~Notification.ForOneDay;
                    doesForOneDay = true;
                    calendarEvent.Notifications = (int)notifications;
                    _eventService.EditEvent(calendarEvent);
                }
                if ((((Notification)calendarEvent.Notifications & Notification.ForTwoDays) == Notification.ForTwoDays) && (calendarEvent.Date == DateTime.Today.AddDays(+2)))
                {
                    forTwoDays += $"● {calendarEvent.Text}\n";
                    notifications &= ~Notification.ForTwoDays;
                    doesForTwoDays = true;
                    calendarEvent.Notifications = (int)notifications;
                    _eventService.EditEvent(calendarEvent);
                }
                if ((((Notification)calendarEvent.Notifications & Notification.ForAWeek) == Notification.ForAWeek) && (calendarEvent.Date == DateTime.Today.AddDays(+7)))
                {
                    forAWeek += $"● {calendarEvent.Text}\n";
                    notifications &= ~Notification.ForAWeek;
                    doesForAWeek = true;
                    calendarEvent.Notifications = (int)notifications;
                    _eventService.EditEvent(calendarEvent);
                }
            }
            if (doesInDay)
                result = "Події на сьогодні:\n" + inDay + "\n";
            if (doesForOneDay)
                result = "Події на завтра:\n" + forOneDay + "\n";
            if (doesForTwoDays)
                result = "Події на післязавтра:\n" + forTwoDays + "\n";
            if (doesForAWeek)
                result = "Події через тиждень:\n" + forAWeek + "\n";
            if (result != "" && result != null)
                await _userService.SendAllNotificatedUsers(bot, result);
        }

        public void StartNotifications(ITelegramBotClient botClient)
        {
            aTimer.Enabled = true;
            bot = botClient;
        }
    }
}
