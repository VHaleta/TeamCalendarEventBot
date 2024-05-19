using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeamCalendarEventBot.Domain.Processor.Handlers
{
    public class UnknownUpdateHandler
    {
        public Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            return Task.CompletedTask;
        }
    }
}
