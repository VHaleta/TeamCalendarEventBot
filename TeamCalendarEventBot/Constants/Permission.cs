using System;
using System.Collections.Generic;
using System.Text;

namespace TeamCalendarEventBot.Constants
{
    [Flags]
    public enum Permission
    {
        Unknown = 0,
        View = 1,
        OwnCalendar = 2,
        CommonCalendar = 4
    }
}
