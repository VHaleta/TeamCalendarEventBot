using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TeamCalendarEventBot.DataStorage.DataModel;

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
        public List<UserInfo> GetAllUsers()
        {
            var dataContents = FileProvider.ReadFile(_fileName);
            if (string.IsNullOrWhiteSpace(dataContents))
            {
                return new List<UserInfo>();
            }

            return JsonConvert.DeserializeObject<List<UserInfo>>(dataContents);
        }

        public UserInfo GetUserInfoById(int userId) => GetAllUsers().FirstOrDefault(x => x.UserId == userId);

        public void UpsertUser(UserInfo userInfo)
        {
            var all = GetAllUsers();

            var exist = all.FirstOrDefault(x => x.UserId == userInfo.UserId);
            if (exist != null)
            {
                all.Remove(exist);
            }
            all.Add(userInfo);

            var newDataContents = JsonConvert.SerializeObject(all.OrderBy(x => x.UserId).ToList());
            SaveData(newDataContents);
        }
        #endregion

        #region Private Methods
        private void SaveData(string contents) => FileProvider.WriteFile(_fileName, contents);
        #endregion
    }
}
