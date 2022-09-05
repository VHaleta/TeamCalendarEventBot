using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCalendarEventBot.Models;

namespace TeamCalendarEventBot.Logger
{
    public interface ILog
    {
        public void LogInfo(string message);
        public void LogError(Exception e, string message);
        public void LogDebug(string message);
    }
}
