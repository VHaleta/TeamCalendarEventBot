using NSubstitute;
using TeamCalendarEventBot.Domain.Processor.Services;
using TeamCalendarEventBot.Domain.Repositories;
using Telegram.Bot;

namespace TeamCalendarEventBot.Tests
{
    public class UserServiceFixture
    {
        public IUserBotRepository UserBotRepository { get; private set; }
        public IMenuService MenuService { get; private set; }
        public ITelegramBotClient TelegramBotClient { get; private set; }

        public UserServiceFixture()
        {
            UserBotRepository = Substitute.For<IUserBotRepository>();
            MenuService = Substitute.For<IMenuService>();
            TelegramBotClient = Substitute.For<ITelegramBotClient>();
        }
    }
}
