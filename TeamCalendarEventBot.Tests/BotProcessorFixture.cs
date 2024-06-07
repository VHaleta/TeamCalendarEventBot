using NSubstitute;
using TeamCalendarEventBot.Domain.Listener;
using TeamCalendarEventBot.Domain.Processor.Handlers;
using TeamCalendarEventBot.Domain.Processor.Services;
using Telegram.Bot;

namespace TeamCalendarEventBot.Tests
{
    public class BotProcessorFixture
    {
        public IUserService UserService { get; private set; }
        public IMessageHandler MessageHandler { get; private set; }
        public ICallbackQueryHandler CallbackQueryHandler { get; private set; }
        public IUnknownUpdateHandler UnknownUpdateHandler { get; private set; }
        public IListener Listener { get; private set; }
        public ITelegramBotClient TelegramBotClient { get; private set; }

        public BotProcessorFixture()
        {
            UserService = Substitute.For<IUserService>();
            MessageHandler = Substitute.For<IMessageHandler>();
            CallbackQueryHandler = Substitute.For<ICallbackQueryHandler>();
            UnknownUpdateHandler = Substitute.For<IUnknownUpdateHandler>();
            Listener = Substitute.For<IListener>();
            TelegramBotClient = Substitute.For<ITelegramBotClient>();
        }
    }
}
