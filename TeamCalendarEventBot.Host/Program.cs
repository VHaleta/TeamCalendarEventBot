using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System.Text;
using TeamCalendarEventBot.Domain.Processor;
using TeamCalendarEventBot.Models.Constants;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TeamCalendarEventBot.Host
{
    internal static class Program
    {
        static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var bot = new TelegramBotClient(TelegramBotInfo.MainToken);

            var config = new ConfigurationBuilder().Build();

            var logger = LogManager.Setup()
                                   .SetupExtensions(ext => ext.RegisterConfigSettings(config))
                                   .GetCurrentClassLogger();

            using var cts = new CancellationTokenSource();

            try
            {
                User me = await bot.GetMeAsync();

                using var servicesProvider = new ServiceCollection()
                    .AddDalDependencies()
                    .AddDomainDependencies()
                    .AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.AddNLog(config);
                    }).BuildServiceProvider();

                var botProcessor = servicesProvider.GetRequiredService<BotProcessor>();

                ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };
                bot.StartReceiving(botProcessor.HandleUpdateAsync,
                                   botProcessor.HandleErrorAsync,
                                   receiverOptions,
                                   cts.Token);

                logger.Debug($"Start listening for @{me.Username}");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
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
}
