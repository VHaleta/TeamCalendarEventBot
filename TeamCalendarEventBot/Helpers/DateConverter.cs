using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCalendarEventBot.Helpers
{
    public static class DateConverter
    {
        public static string NumberToMonth(int month)
        {
            switch (month)
            {
                case 1: return "Січень";
                case 2: return "Лютий";
                case 3: return "Березень";
                case 4: return "Квітень";
                case 5: return "Травень";
                case 6: return "Червень";
                case 7: return "Липень";
                case 8: return "Серпень";
                case 9: return "Вересень";
                case 10: return "Жовтень";
                case 11: return "Листопад";
                case 12: return "Грудень";
                    default: return "unknown";
            }
        }

        public static string EngToRusDay(string day)
        {
            //
            day = day.ToLower();
            switch (day)
            {
                case "monday":
                    return "понеділок";
                case "tuesday":
                    return "вівторок";
                case "wednesday":
                    return "середу";
                case "thursday":
                    return "четвер";
                case "friday":
                    return "п'ятницю";
                case "saturday":
                    return "суботу";
                case "sunday":
                    return "неділю";
                    default : return "unknown";
            }
        }
    }
}
