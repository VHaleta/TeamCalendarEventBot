using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCalendarEventBot.Constants;
using TeamCalendarEventBot.Helpers;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeamCalendarEventBot.Services
{
    static class Calendar
    {
        //TODO: Point only on current date
        public static InlineKeyboardMarkup GetCalendarKeyboard(DateTime date)
        {
            int firstDay = 1, day = 1, daysInMonth;
            List<List<string>> days = new List<List<string>>();
            daysInMonth = DateTime.DaysInMonth(date.Day, date.Month);

            for (; firstDay < 7; firstDay++) //find first day
            {
                if (((int)new DateTime(date.Year, date.Month, 1).DayOfWeek) == firstDay) break;
            }
            days.Add(new List<string>());
            for (int i = 0; i < firstDay - 1; i++) //fill before first day
            {
                days[days.Count - 1].Add(" ");
            }
            for (; day <= daysInMonth; day++) //fill main
            {
                if (days[days.Count - 1].Count == 7)
                    days.Add(new List<string>());
                days[days.Count - 1].Add(day.ToString());
            }
            for (int i = days[days.Count - 1].Count; i < 7; i++) //fill after last day
            {
                days[days.Count - 1].Add(" ");
            }

            return GetCalendar(days, date);
        }

        static InlineKeyboardMarkup GetCalendar(List<List<string>> days, DateTime date)
        {
            string endCallback = "." + ZeroAdder.AddZero(date.Month) + "." + date.Year.ToString();
            List<List<InlineKeyboardButton>> keyboardButtons = new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>{ new InlineKeyboardButton("<") {CallbackData = $"{CallbackConst.ChangeMonth} {date.Month - 1}"}, new InlineKeyboardButton(MonthConverter.NumberToText(date.Month)) { CallbackData = "nothing" }, new InlineKeyboardButton(">") { CallbackData = $"{CallbackConst.ChangeMonth} {date.Month + 1}" } },
                    new List<InlineKeyboardButton>{new InlineKeyboardButton("ПН") { CallbackData = "nothing" }, new InlineKeyboardButton("ВТ") { CallbackData = "nothing" }, new InlineKeyboardButton("СР") { CallbackData = "nothing" }, new InlineKeyboardButton("ЧТ") { CallbackData = "nothing" }, new InlineKeyboardButton("ПТ") { CallbackData = "nothing" }, new InlineKeyboardButton("СБ") { CallbackData = "nothing" }, new InlineKeyboardButton("ВС") { CallbackData = "nothing" } } ,
                };
            for (int i = 0; i < days.Count; i++)
            {
                keyboardButtons.Add(new List<InlineKeyboardButton> { });
                for (int j = 0; j < 7; j++)
                {
                    keyboardButtons[i + 2].Add(new InlineKeyboardButton(days[i][j] + ((days[i][j] == date.Day.ToString()) ? "." : "")) { CallbackData = (days[i][j] == " ") ? "nothing" : ZeroAdder.AddZero(int.Parse(days[i][j])) + endCallback });
                }
            }
            return new InlineKeyboardMarkup(keyboardButtons);
        }
    }
}
