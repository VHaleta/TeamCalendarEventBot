using System.Collections.Generic;
using TeamCalendarEventBot.DataStorage.DataModel;

namespace TeamCalendarEventBot.DataStorage
{
    public interface IUserInfoDataProvider
    {
        List<UserInfo> GetAllUsers();
        UserInfo GetUserInfoById(int userId);
        void UpsertUser(UserInfo userInfo);
    }
}
