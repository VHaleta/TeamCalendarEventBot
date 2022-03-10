using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCalendarEventBot.Helpers
{
    public static class MonthConverter
    {
        public static string NumberToText(int month)
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
    }
}
