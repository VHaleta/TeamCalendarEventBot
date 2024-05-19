using TeamCalendarEventBot.Domain.Helpers;
using TeamCalendarEventBot.Models.Constants;
using TeamCalendarEventBot.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeamCalendarEventBot.Domain.Processor.Services
{
    public class CalendarService
    {
        private readonly EventService _eventService;

        public CalendarService(EventService eventService)
        {
            _eventService = eventService;
        }

        public InlineKeyboardMarkup GetCalendarKeyboard(DateTime date, string callback)
        {
            List<List<string>> days;
            string prev = callback;
            Days(date, out days);
            return GetCalendarInlineKeyboardMarkup(date, days, prev);
        }

        private void Days(DateTime date, out List<List<string>> days)
        {
            int firstDay = 1, day = 1, daysInMonth;
            days = new List<List<string>>();
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
        }
        private InlineKeyboardMarkup GetCalendarInlineKeyboardMarkup(DateTime date, List<List<string>> days, string prev)
        {
            var prevMonth = date.AddMonths(-1);
            var nextMonth = date.AddMonths(1);
            List<List<InlineKeyboardButton>> keyboardButtons = new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>{ new InlineKeyboardButton("<") { CallbackData = $"{CallbackConst.ChangeMonth} {prev} {prevMonth.Month} {prevMonth.Year}"}, new InlineKeyboardButton(DateConverter.NumberToMonth(date.Month)) { CallbackData = CallbackConst.Nothing }, new InlineKeyboardButton(">") { CallbackData = $"{CallbackConst.ChangeMonth} {prev} {nextMonth.Month} {nextMonth.Year}" } },
                    new List<InlineKeyboardButton>{ new InlineKeyboardButton("ПН") { CallbackData = CallbackConst.Nothing }, new InlineKeyboardButton("ВТ") { CallbackData = CallbackConst.Nothing }, new InlineKeyboardButton("СР") { CallbackData = CallbackConst.Nothing }, new InlineKeyboardButton("ЧТ") { CallbackData = CallbackConst.Nothing }, new InlineKeyboardButton("ПТ") { CallbackData = CallbackConst.Nothing }, new InlineKeyboardButton("СБ") { CallbackData = CallbackConst.Nothing }, new InlineKeyboardButton("НД") { CallbackData = CallbackConst.Nothing } } ,
                };
            for (int i = 0; i < days.Count; i++)
            {
                keyboardButtons.Add(new List<InlineKeyboardButton> { });
                int day, eventsCount;
                for (int j = 0; j < 7; j++)
                {
                    if (int.TryParse(days[i][j], out day))
                    {
                        DateTime tempDate = new DateTime(date.Year, date.Month, day);
                        eventsCount = _eventService.CountCalendarEventsByDate(tempDate);
                        keyboardButtons[i + 2].Add(new InlineKeyboardButton($"{days[i][j]}{((eventsCount > 0) ? $"({eventsCount})" : "")}{((tempDate == DateTime.Today) ? "!" : "")}") { CallbackData = $"{prev} {tempDate.ToString("dd.MM.yyyy")}" });
                    }
                    else
                        keyboardButtons[i + 2].Add(new InlineKeyboardButton(" ") { CallbackData = CallbackConst.Nothing });
                }
            }

            return new InlineKeyboardMarkup(keyboardButtons);

        }
    }
}
