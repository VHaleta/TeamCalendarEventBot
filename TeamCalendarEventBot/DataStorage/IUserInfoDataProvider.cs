using System.Collections.Generic;
using TeamCalendarEventBot.Models;

namespace TeamCalendarEventBot.DataStorage
{
    public interface IUserInfoDataProvider
    {
        List<UserBot> GetAllUsers();
        void UpsertUser(UserBot userBot);
    }
}
