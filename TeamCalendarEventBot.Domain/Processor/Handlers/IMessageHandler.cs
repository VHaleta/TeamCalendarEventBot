using TeamCalendarEventBot.Models.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeamCalendarEventBot.Domain.Processor.Handlers
{
    public interface IMessageHandler
    {
        public Task BotOnMessageReceivedAsync(ITelegramBotClient botClient, Message message, UserBot user);
    }
}
