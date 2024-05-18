using TeamCalendarEventBot.Models.Models;

namespace TeamCalendarEventBot.Domain.Repositories
{
    public interface IUserBotRepository
    {
        IEnumerable<UserBot> GetAllUsers();
        void UpsertUser(UserBot userBot);
    }
}
