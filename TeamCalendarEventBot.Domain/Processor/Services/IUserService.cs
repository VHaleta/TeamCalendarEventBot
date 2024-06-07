using TeamCalendarEventBot.Models.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeamCalendarEventBot.Domain.Processor.Services
{
    public interface IUserService
    {
        public Task<bool> IsUserAuthorizedAsync(ITelegramBotClient botClient, Message message, UserBot user);

        public UserBot GetUser(Update update);

        public void UpdateUser(UserBot user);

        public List<UserBot> GetAllRequestedUsers();

        public UserBot FindUser(long chatId);

        public List<UserBot> GetAllUsersExceptMe(UserBot user);

        public Task SendAllUsers(ITelegramBotClient botClient, string messageText);

        public Task SendAllNotificatedUsers(ITelegramBotClient botClient, string messageText);
    }
}
