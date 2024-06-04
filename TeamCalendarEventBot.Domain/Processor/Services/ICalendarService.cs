using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeamCalendarEventBot.Domain.Processor.Services
{
    public interface ICalendarService
    {
        public InlineKeyboardMarkup GetCalendarKeyboard(DateTime date, string callback);
    }
}
