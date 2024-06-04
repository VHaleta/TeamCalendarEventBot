using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TeamCalendarEventBot.Domain.Listener
{
    public class Listener : IListener
    {
        private readonly ISqsCommunication _sqsCommunication;

        public Listener(ILogger<Listener> logger, ISqsCommunication sqsCommunication)
        {
            _sqsCommunication = sqsCommunication;
        }

        public async ValueTask OnApiRequest(ITelegramBotClient botClient, ApiRequestEventArgs apiRequestEventArgs, CancellationToken cancellationToken)
        {
            string message = await apiRequestEventArgs.HttpRequestMessage.Content.ReadAsStringAsync(cancellationToken);
            DateTimeOffset date = apiRequestEventArgs.HttpRequestMessage.Headers.Date ?? DateTime.Now;
            string context = "OnApiRequest " + apiRequestEventArgs.Request.MethodName;

            await _sqsCommunication.Send(context, date, message);
        }
    }
}
