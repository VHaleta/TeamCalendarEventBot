using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Telegram.Bot;
using TeamCalendarEventBot.Models;
using TeamCalendarEventBot.Constants;

namespace TeamCalendarEventBot.Services
{
    public static class NotificationHandler
    {
        private static Timer aTimer;
        private static ITelegramBotClient bot;
        static NotificationHandler()
        {
            aTimer = new Timer(5000);
            aTimer.Elapsed += CheckNotifications;
        }
        private static async void CheckNotifications(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("NOTIFICATION");
            List<CalendarEvent> calendarEvents = EventHandler.GetCalendarEventsForNotification();
            string result = "", inDay = "", forOneDay = "", forTwoDays = "", forAWeek = "";
            bool doesInDay = false, doesForOneDay = false, doesForTwoDays = false, doesForAWeek = false;
            foreach (var calendarEvent in calendarEvents)
            {
                Notification notifications = (Notification)calendarEvent.Notifications;
                if ((((Notification)calendarEvent.Notifications & Notification.InDay) == Notification.InDay) && (calendarEvent.Date == DateTime.Today))
                {
                    inDay = $"● {calendarEvent.Text}\n";
                    notifications &= ~Notification.InDay;
                    doesInDay = true;
                    calendarEvent.Notifications = (int)notifications;
                    EventHandler.EditEvent(calendarEvent);
                }
                if ((((Notification)calendarEvent.Notifications & Notification.ForOneDay) == Notification.ForOneDay) && (calendarEvent.Date == DateTime.Today.AddDays(+1)))
                {
                    forOneDay = $"● {calendarEvent.Text}\n";
                    notifications &= ~Notification.ForOneDay;
                    doesForOneDay = true;
                    calendarEvent.Notifications = (int)notifications;
                    EventHandler.EditEvent(calendarEvent);
                }
                if ((((Notification)calendarEvent.Notifications & Notification.ForTwoDays) == Notification.ForTwoDays) && (calendarEvent.Date == DateTime.Today.AddDays(+2)))
                {
                    forTwoDays = $"● {calendarEvent.Text}\n";
                    notifications &= ~Notification.ForTwoDays;
                    doesForTwoDays = true;
                    calendarEvent.Notifications = (int)notifications;
                    EventHandler.EditEvent(calendarEvent);
                }
                if ((((Notification)calendarEvent.Notifications & Notification.ForAWeek) == Notification.ForAWeek) && (calendarEvent.Date == DateTime.Today.AddDays(+7)))
                {
                    forAWeek = $"● {calendarEvent.Text}\n";
                    notifications &= ~Notification.ForAWeek;
                    doesForAWeek = true;
                    calendarEvent.Notifications = (int)notifications;
                    EventHandler.EditEvent(calendarEvent);
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
                await bot.SendTextMessageAsync(500661841, result);
        }

        public static void StartNotifications(ITelegramBotClient botClient)
        {
            aTimer.Enabled = true;
            bot = botClient;
        }


    }
}
