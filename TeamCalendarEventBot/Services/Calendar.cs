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
        //TODO: cahnge dd.mm.yyyy
        static InlineKeyboardMarkup GetCalendar(List<List<string>> days, DateTime date)
        {
            var prevMonth = date.AddMonths(-1);
            var nextMonth = date.AddMonths(1);
            string endCallback = "." + ZeroAdder.AddZero(date.Month) + "." + date.Year.ToString();
            List<List<InlineKeyboardButton>> keyboardButtons = new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>{ new InlineKeyboardButton("<") { CallbackData = $"{CallbackConst.ChangeMonth} {prevMonth.Month} {prevMonth.Year}"}, new InlineKeyboardButton(MonthConverter.NumberToText(date.Month)) { CallbackData = CallbackConst.Nothing }, new InlineKeyboardButton(">") { CallbackData = $"{CallbackConst.ChangeMonth} {nextMonth.Month} {nextMonth.Year}" } },
                    new List<InlineKeyboardButton>{ new InlineKeyboardButton("ПН") { CallbackData = CallbackConst.Nothing }, new InlineKeyboardButton("ВТ") { CallbackData = CallbackConst.Nothing }, new InlineKeyboardButton("СР") { CallbackData = CallbackConst.Nothing }, new InlineKeyboardButton("ЧТ") { CallbackData = CallbackConst.Nothing }, new InlineKeyboardButton("ПТ") { CallbackData = CallbackConst.Nothing }, new InlineKeyboardButton("СБ") { CallbackData = CallbackConst.Nothing }, new InlineKeyboardButton("ВС") { CallbackData = CallbackConst.Nothing } } ,
                };
            for (int i = 0; i < days.Count; i++)
            {
                keyboardButtons.Add(new List<InlineKeyboardButton> { });
                for (int j = 0; j < 7; j++)
                {
                    keyboardButtons[i + 2].Add(new InlineKeyboardButton(days[i][j] + ((days[i][j] == DateTime.Today.Day.ToString() && date.Month == DateTime.Today.Month) ? "." : "")) { CallbackData = (days[i][j] == " ") ? "nothing" : ZeroAdder.AddZero(int.Parse(days[i][j])) + endCallback });
                }
            }
            return new InlineKeyboardMarkup(keyboardButtons);
        }
    }
}
