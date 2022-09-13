using System;
using TeamCalendarEventBot.Models;

namespace TeamCalendarEventBot.Logger
{
    public static class LogHandler
    {
        private static readonly ILog logger;
        static LogHandler()
        {
            logger = new Log();
        }

        public static void LogDebug(string message, UserBot user)
        {
            message += $" username = {user.Username}({user.FirstName} {user.LastName})";
            logger.LogDebug(message);
        }

        public static void LogDebug(string message)
        {
            logger.LogDebug(message);
        }

        public static void LogInfo(string message, UserBot user)
        {
            message += $" username = {user.Username}({user.FirstName} {user.LastName})";
            logger.LogInfo(message);
        }

        public static void LogInfo(string message)
        {
            logger.LogInfo(message);
        }

        public static void LogError(Exception e, string message, UserBot user)
        {
            message += $" username = {user.Username}({user.FirstName} {user.LastName})";
            logger.LogError(e, message);
        }

        public static void LogError(Exception e, string message)
        {
            logger.LogError(e, message);
        }
    }
}
