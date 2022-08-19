using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TeamCalendarEventBot.Services
{
    public static class NotificationHandler
    {
        private static Timer aTimer;
        private static ITelegramBotClient botClient;
        static NotificationHandler()
        {
            aTimer = new Timer(30000);
            aTimer.Elapsed += CheckNotifications;
        }
        private static void CheckNotifications(Object source, ElapsedEventArgs e)
        {

        }

        public static void StartNotifications(ITelegramBotClient botClient)
        {
            aTimer.Enabled = true;
        }
    }
}
