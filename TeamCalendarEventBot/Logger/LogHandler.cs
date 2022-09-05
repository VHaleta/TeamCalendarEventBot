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
            message += $" username = {user.Username} userStatus = {user.UserStatus}";
            logger.LogDebug(message);
        }

        public static void LogDebug(string message)
        {
            logger.LogDebug(message);
        }

        public static void LogInfo(string message, UserBot user)
        {
            message += $" username = {user.Username} userStatus = {user.UserStatus}";
            logger.LogInfo(message);
        }

        public static void LogInfo(string message)
        {
            logger.LogInfo(message);
        }

        public static void LogError(Exception e, string message, UserBot user)
        {
            message += $" username = {user.Username} userStatus = {user.UserStatus}";
            logger.LogError(e, message);
        }

        public static void LogError(Exception e, string message)
        {
            logger.LogError(e, message);
        }
    }
}
