using System;
using System.Threading;
using System.Threading.Tasks;
using TeamCalendarEventBot.DataStorage.DataJsonFile;
using TeamCalendarEventBot.Sevices;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TeamCalendarEventBot.Constants;

namespace TeamCalendarEventBot.Services
{
    public static class BotProcessor
    {
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var user = UserHandler.GetUser(update);
            if (!user.Active) return;

            // TODO: Check Active -> Ignore
            // TODO: Auth -> NONE -> Suggest to join ->  button
            // TODO: Auth -> Requsted -> send message 

            var handler = update.Type switch
            {
                // UpdateType.Unknown:
                // UpdateType.ChannelPost:
                // UpdateType.EditedChannelPost:
                // UpdateType.ShippingQuery:
                // UpdateType.PreCheckoutQuery:
                // UpdateType.Poll:
                //UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
                //UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery!),
                //UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult!),

                UpdateType.Message => UpdateHandler.BotOnMessageReceivedAsync(botClient, update.Message, user),
                //UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!),
                _ => UpdateHandler.UnknownUpdateHandlerAsync(botClient, update)

            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

    }
}
