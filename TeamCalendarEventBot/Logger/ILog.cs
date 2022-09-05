using System;

namespace TeamCalendarEventBot.Logger
{
    public interface ILog
    {
        public void LogInfo(string message);
        public void LogError(Exception e, string message);
        public void LogDebug(string message);
    }
}
