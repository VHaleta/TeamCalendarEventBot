using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TeamCalendarEventBot.Models;

namespace TeamCalendarEventBot.DataStorage.DataJsonFile
{
    public class UserInfoDataProvider : JsonFileDataProviderBase, IUserInfoDataProvider
    {
        #region Fields
        private const string _fileName = "users.dat";
        #endregion

        #region Constructors
        public UserInfoDataProvider(IFileProvider fileProvider) : base(fileProvider)
        {
        }
        #endregion

        #region Public Methods
        public List<UserBot> GetAllUsers()
        {
            var dataContents = FileProvider.ReadFile(_fileName);
            if (string.IsNullOrWhiteSpace(dataContents))
            {
                return new List<UserBot>();
            }

            return JsonConvert.DeserializeObject<List<UserBot>>(dataContents);
        }

        public UserBot GetUserInfoById(int chatId) => GetAllUsers().FirstOrDefault(x => x.ChatId == chatId);

        public void UpsertUser(UserBot userBot)
        {
            var all = GetAllUsers();

            var exist = all.FirstOrDefault(x => x.UserId == userBot.UserId);
            if (exist != null)
            {
                all.Remove(exist);
            }
            all.Add(userBot);

            all = all.OrderBy(x => x.UserId).ToList();
            var newDataContents = JsonConvert.SerializeObject(all);
            FileProvider.WriteFile(_fileName, newDataContents);
        }
        #endregion
    }
}
