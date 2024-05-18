using TeamCalendarEventBot.DAL.DataModels;

namespace TeamCalendarEventBot.DAL.Persistence
{
    public interface IUserBotDao
    {
        IEnumerable<UserBotData> GetAllUsers();

        void UpsertUser(UserBotData userBot);
    }
}
