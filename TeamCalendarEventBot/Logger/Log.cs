using System;
using TeamCalendarEventBot.Models;

namespace TeamCalendarEventBot.Logger
{
    public class Log : ILog
    {
        private static NLog.Logger logger;
        public Log()
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public void LogDebug(string message)
        {
            logger.Debug(message);
        }

        public void LogError(Exception e, string message)
        {
            logger.Error(e, message);
        }

        public void LogInfo(string message)
        {
            logger.Info(message);
        }
    }
}
