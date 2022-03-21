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
                case 1: return "Январь/Січень";
                case 2: return "Февраль/Лютий";
                case 3: return "Март/Березень";
                case 4: return "Апрель/Квітень";
                case 5: return "Май/Травень";
                case 6: return "Июнь/Червень";
                case 7: return "Июль/Липень";
                case 8: return "Август/Серпень";
                case 9: return "Сентябрь/Вересень";
                case 10: return "Октябрь/Жовтень";
                case 11: return "Ноябрь/Листопад";
                case 12: return "Декабрь/Грудень";
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
                    return "понедельник";
                case "tuesday":
                    return "вторник";
                case "wednesday":
                    return "среду";
                case "thursday":
                    return "четверг";
                case "friday":
                    return "пятницу";
                case "saturday":
                    return "субботу";
                case "sunday":
                    return "воскресенье";
                    default : return "unknown";
            }
        }
    }
}
