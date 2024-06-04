using TeamCalendarEventBot.Models.Constants;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeamCalendarEventBot.Domain.Processor.Services
{
    public interface IMenuService
    {
        public ReplyKeyboardMarkup GetMenuButtons(Permission permission, MenuStage menuStage);

        public ReplyKeyboardMarkup NoneAuthKeybord();
    }
}
