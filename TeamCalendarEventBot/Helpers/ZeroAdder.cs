using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCalendarEventBot.Helpers
{
    public static class ZeroAdder
    {
        public static string AddZero(int nomber) =>
            (nomber / 10 > 0) ? nomber.ToString() : "0" + nomber.ToString();
    }
}
