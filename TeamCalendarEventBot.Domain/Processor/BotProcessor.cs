using Microsoft.Extensions.Logging;
using TeamCalendarEventBot.Domain.Listener;
using TeamCalendarEventBot.Domain.Processor.Handlers;
using TeamCalendarEventBot.Domain.Processor.Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeamCalendarEventBot.Domain.Processor
{
    public class BotProcessor
    {
        private static bool isAssigned = false;
        private readonly IUserService _userService;
        private readonly ILogger<BotProcessor> _logger;
        private readonly MessageHandler _messageHandler;
        private readonly CallbackQueryHandler _callbackQueryHandler;
        private readonly UnknownUpdateHandler _unknownUpdateHandler;
        private readonly IListener _listener;

        public BotProcessor(
            IUserService userService,
            ILogger<BotProcessor> logger,
            MessageHandler messageHandler,
            CallbackQueryHandler callbackQueryHandler,
            UnknownUpdateHandler unknownUpdateHandler,
            IListener listener)
        {
            _userService = userService;
            _logger = logger;
            _messageHandler = messageHandler;
            _callbackQueryHandler = callbackQueryHandler;
            _unknownUpdateHandler = unknownUpdateHandler;
            _listener = listener;
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.Message,
            };

            _logger.LogError(exception, ErrorMessage);
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (!isAssigned)
            {
                botClient.OnMakingApiRequest += _listener.OnApiRequest;
                isAssigned = true;
            }
            var user = _userService.GetUser(update);
            if (!user.Active)
            {
                _logger.LogDebug($"User is unactive", user);
                return;
            }
            if (!_userService.IsUserAuthorizedAsync(botClient, update.Message, user).Result)
            {
                _logger.LogDebug($"User isn`t authorized", user);
                return;
            }

            Task handler;
            switch (update.Type)
            {
                case UpdateType.Message:
                    _logger.LogInformation($"Recieved update {update.Type}: {update.Message.Text}", user);
                    handler = _messageHandler.BotOnMessageReceivedAsync(botClient, update.Message, user);
                    break;
                case UpdateType.CallbackQuery:
                    _logger.LogInformation($"Recieved update {update.Type}: {update.CallbackQuery.Data}", user);
                    handler = _callbackQueryHandler.BotOnCallbackQueryReceived(botClient, update.CallbackQuery, user);
                    break;
                default:
                    _logger.LogInformation($"Recieved unknown update type: {update.Type}", user);
                    handler = _unknownUpdateHandler.UnknownUpdateHandlerAsync(botClient, update);
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
