using System;

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
