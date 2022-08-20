using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCalendarEventBot.Constants
{
    [Flags]
    public enum Notification
    {
        No = 0,
        InDay = 1,
        ForOneDay = 2,
        ForTwoDays = 4,
        ForAWeek = 8,
    }
}
