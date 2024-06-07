using TeamCalendarEventBot.Models.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeamCalendarEventBot.Domain.Processor.Handlers
{
    public interface ICallbackQueryHandler
    {
        public Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user);
    }
}
