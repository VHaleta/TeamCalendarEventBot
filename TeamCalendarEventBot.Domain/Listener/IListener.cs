using Telegram.Bot;
using Telegram.Bot.Args;

namespace TeamCalendarEventBot.Domain.Listener
{
    public interface IListener
    {
        ValueTask OnApiRequest(ITelegramBotClient botClient, ApiRequestEventArgs apiRequestEventArgs, CancellationToken cancellationToken);
    }
}
