using System.Collections.Generic;
using TeamCalendarEventBot.Models;

namespace TeamCalendarEventBot.DataStorage
{
    public interface IUserInfoDataProvider
    {
        List<UserBot> GetAllUsers();
        UserBot GetUserInfoById(int chatId);
        void UpsertUser(UserBot userBot);
    }
}
