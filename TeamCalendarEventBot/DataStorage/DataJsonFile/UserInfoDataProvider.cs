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
        private static object _locker = new object();
        #endregion

        #region Constructors
        public UserInfoDataProvider(IFileProvider fileProvider) : base(fileProvider)
        {
        }
        #endregion

        #region Public Methods
        public List<UserBot> GetAllUsers()
        {
            lock (_locker)
            {
                var dataContents = FileProvider.ReadFile(_fileName);
                if (string.IsNullOrWhiteSpace(dataContents))
                {
                    return new List<UserBot>();
                }

                return JsonConvert.DeserializeObject<List<UserBot>>(dataContents);
            }
        }

        public void UpsertUser(UserBot userBot)
        {
            lock (_locker)
            {
                var all = GetAllUsers();
                var exist = all.FirstOrDefault(x => x.ChatId == userBot.ChatId);
                if (exist != null)
                {
                    all.Remove(exist);
                }
                all.Add(userBot);

                all = all.OrderBy(x => x.UserId).ToList();
                var newDataContents = JsonConvert.SerializeObject(all);
                FileProvider.WriteFile(_fileName, newDataContents);
            }
        }
        #endregion
    }
}
