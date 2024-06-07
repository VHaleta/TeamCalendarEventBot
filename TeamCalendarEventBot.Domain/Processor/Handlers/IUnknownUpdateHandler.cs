using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeamCalendarEventBot.Domain.Processor.Handlers
{
    public interface IUnknownUpdateHandler
    {
        public Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update);
    }
}
