using System;
using System.Threading;
using System.Threading.Tasks;
using TeamCalendarEventBot.Sevices;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TeamCalendarEventBot.Logger;

namespace TeamCalendarEventBot.Services
{
    public static class BotProcessor
    {
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.Message,
            };

            LogHandler.LogError(exception, ErrorMessage);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var user = UserHandler.GetUser(update);
            if (!user.Active)
            {
                LogHandler.LogDebug($"User is unactive", user);
                return;
            }
            if (!UserHandler.IsUserAuthorizedAsync(botClient, update.Message, user).Result)
            {
                LogHandler.LogDebug($"User isn`t authorized", user);
                return;
            }

            Task handler;
            switch (update.Type)
            {
                case UpdateType.Message:
                    LogHandler.LogInfo($"Recieved update {update.Type}: {update.Message.Text}", user);
                    handler = UpdateHandler.BotOnMessageReceivedAsync(botClient, update.Message, user);
                    break;
                case UpdateType.CallbackQuery:
                    LogHandler.LogInfo($"Recieved update {update.Type}: {update.CallbackQuery.Data}", user);
                    handler = UpdateHandler.BotOnCallbackQueryReceived(botClient, update.CallbackQuery, user);
                    break;
                default:
                    LogHandler.LogInfo($"Recieved unknown update type: {update.Type}", user);
                    handler = UpdateHandler.UnknownUpdateHandlerAsync(botClient, update);
                    break;
            }

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
