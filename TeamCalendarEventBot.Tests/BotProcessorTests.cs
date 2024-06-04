using AutoFixture;
using FluentAssertions;
using NSubstitute;
using TeamCalendarEventBot.Domain.Processor;
using TeamCalendarEventBot.Domain.Processor.Handlers;
using TeamCalendarEventBot.Domain.Processor.Services;
using TeamCalendarEventBot.Models.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;

namespace TeamCalendarEventBot.Tests
{
    public class BotProcessorTests : IClassFixture<BotProcessorFixture>
    {
        private readonly BotProcessor botProcessor;
        private readonly MessageHandler messageHandlerSubstitute;
        private readonly CallbackQueryHandler callbackQueryHandlerSubstitute;
        private readonly UnknownUpdateHandler unknownUpdateHandlerSubstitute;
        private readonly IUserService userServiceSubstitute;
        private readonly ITelegramBotClient telegramBotClient;
        private readonly Fixture fixture;

        public BotProcessorTests(BotProcessorFixture botProcessorFixture)
        {
            messageHandlerSubstitute = botProcessorFixture.MessageHandler;
            callbackQueryHandlerSubstitute = botProcessorFixture.CallbackQueryHandler;
            unknownUpdateHandlerSubstitute = botProcessorFixture.UnknownUpdateHandler;
            telegramBotClient = botProcessorFixture.TelegramBotClient;
            userServiceSubstitute = botProcessorFixture.UserService;

            botProcessor = new BotProcessor(
                botProcessorFixture.UserService,
                botProcessorFixture.Logger,
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
            var update = Substitute.For<Update>();
            update.Type.Returns(UpdateType.Unknown);

            var user = fixture.Create<UserBot>();
            userServiceSubstitute.GetUser(new Update()).ReturnsForAnyArgs(user);
            userServiceSubstitute.IsUserAuthorizedAsync(telegramBotClient, new Message(), user).ReturnsForAnyArgs(true);


            //Act
            await botProcessor.HandleUpdateAsync(telegramBotClient, update, CancellationToken.None);

            //Assert
            var receivedCalls = unknownUpdateHandlerSubstitute.UnknownUpdateHandlerAsync(telegramBotClient, update).ReceivedCalls();
            receivedCalls.Should().HaveCount(1);
        }
    }
}