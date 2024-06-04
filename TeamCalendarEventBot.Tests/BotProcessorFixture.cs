using Microsoft.Extensions.Logging;
using NSubstitute;
using TeamCalendarEventBot.Domain.Listener;
using TeamCalendarEventBot.Domain.Processor;
using TeamCalendarEventBot.Domain.Processor.Handlers;
using TeamCalendarEventBot.Domain.Processor.Services;
using Telegram.Bot;

namespace TeamCalendarEventBot.Tests
{
    public class BotProcessorFixture
    {
        public IUserService UserService { get; private set; }
        public ILogger<BotProcessor> Logger { get; private set; }
        public MessageHandler MessageHandler { get; private set; }
        public CallbackQueryHandler CallbackQueryHandler { get; private set; }
        public UnknownUpdateHandler UnknownUpdateHandler { get; private set; }
        public IListener Listener { get; private set; }
        public ITelegramBotClient TelegramBotClient { get; private set; }

        public BotProcessorFixture()
        {
            UserService = Substitute.For<IUserService>();
            Logger = Substitute.For<ILogger<BotProcessor>>();
            MessageHandler = Substitute.For<MessageHandler>();
            CallbackQueryHandler = Substitute.For<CallbackQueryHandler>();
            UnknownUpdateHandler = Substitute.For<UnknownUpdateHandler>();
            Listener = Substitute.For<IListener>();
            TelegramBotClient = Substitute.For<ITelegramBotClient>();
        }
    }
}
