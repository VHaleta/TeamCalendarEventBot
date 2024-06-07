using AutoFixture;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using TeamCalendarEventBot.Domain.Processor;
using TeamCalendarEventBot.Domain.Processor.Handlers;
using TeamCalendarEventBot.Domain.Processor.Services;
using TeamCalendarEventBot.Models.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Xunit;

namespace TeamCalendarEventBot.Tests
{
    public class BotProcessorTests : IClassFixture<BotProcessorFixture>, IClassFixture<LoggerFixture<BotProcessor>>
    {
        private readonly BotProcessor botProcessor;
        private readonly IMessageHandler messageHandlerSubstitute;
        private readonly ICallbackQueryHandler callbackQueryHandlerSubstitute;
        private readonly IUnknownUpdateHandler unknownUpdateHandlerSubstitute;
        private readonly IUserService userServiceSubstitute;
        private readonly ITelegramBotClient telegramBotClient;
        private readonly Fixture fixture;

        public BotProcessorTests(BotProcessorFixture botProcessorFixture, LoggerFixture<BotProcessor> loggerFixture)
        {
            messageHandlerSubstitute = botProcessorFixture.MessageHandler;
            callbackQueryHandlerSubstitute = botProcessorFixture.CallbackQueryHandler;
            unknownUpdateHandlerSubstitute = botProcessorFixture.UnknownUpdateHandler;
            telegramBotClient = botProcessorFixture.TelegramBotClient;
            userServiceSubstitute = botProcessorFixture.UserService;

            botProcessor = new BotProcessor(
                botProcessorFixture.UserService,
                loggerFixture.Logger,
                botProcessorFixture.MessageHandler,
                botProcessorFixture.CallbackQueryHandler,
                botProcessorFixture.UnknownUpdateHandler,
                botProcessorFixture.Listener);

            fixture = new Fixture();
        }

        [Fact]
        public async void HandleUpdateAsync_WithUnknownUpdateType_CallsUnknownUpdateHandler()
        {
            // Arrange
            var update = new Update();

            var user = fixture.Create<UserBot>();
            userServiceSubstitute.GetUser(new Update()).ReturnsForAnyArgs(user);
            userServiceSubstitute.IsUserAuthorizedAsync(telegramBotClient, new Message(), user).ReturnsForAnyArgs(true);


            //Act
            await botProcessor.HandleUpdateAsync(telegramBotClient, update, CancellationToken.None);

            //Assert
            unknownUpdateHandlerSubstitute.Received(1);
        }

        [Fact]
        public async void HandleUpdateAsync_WithCallbackQueryUpdateType_CallsCallbackQueryHandler()
        {
            // Arrange
            var update = new Update()
            {
                CallbackQuery = new CallbackQuery()
            };

            var user = fixture.Create<UserBot>();
            userServiceSubstitute.GetUser(new Update()).ReturnsForAnyArgs(user);
            userServiceSubstitute.IsUserAuthorizedAsync(telegramBotClient, new Message(), user).ReturnsForAnyArgs(true);


            //Act
            await botProcessor.HandleUpdateAsync(telegramBotClient, update, CancellationToken.None);

            //Assert
            callbackQueryHandlerSubstitute.Received(1);
        }

        [Fact]
        public async void HandleUpdateAsync_WithMessageUpdateType_CallsMessageHandler()
        {
            // Arrange
            var update = new Update()
            {
                Message = new Message()
            };

            var user = fixture.Create<UserBot>();
            userServiceSubstitute.GetUser(new Update()).ReturnsForAnyArgs(user);
            userServiceSubstitute.IsUserAuthorizedAsync(telegramBotClient, new Message(), user).ReturnsForAnyArgs(true);


            //Act
            await botProcessor.HandleUpdateAsync(telegramBotClient, update, CancellationToken.None);

            //Assert
            messageHandlerSubstitute.Received(1);
        }
    }
}