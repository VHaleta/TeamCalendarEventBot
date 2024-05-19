using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeamCalendarEventBot.Constants;
using TeamCalendarEventBot.Logger;
using TeamCalendarEventBot.Services;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace TeamCalendarEventBot
{
    public static class Program
    {
        private static TelegramBotClient Bot;

        public static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Bot = new TelegramBotClient(TelegramBotInfo.MainToken);

            User me = await Bot.GetMeAsync();

            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };
            Bot.StartReceiving(BotProcessor.HandleUpdateAsync,
                               BotProcessor.HandleErrorAsync,
                               receiverOptions,
                               cts.Token);

            LogHandler.LogDebug($"Start listening for @{me.Username}");

#if DEBUG
            // To Debug in Windows Console
            Console.ReadLine();
#else
            // To run as daemon in Linux
            Thread.Sleep(-1);
#endif
            // Send cancellation request to stop bot
            cts.Cancel();
        }
    }
}
