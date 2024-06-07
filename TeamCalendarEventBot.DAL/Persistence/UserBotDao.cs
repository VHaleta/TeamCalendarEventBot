using Newtonsoft.Json;
using TeamCalendarEventBot.DAL.DataModels;
using TeamCalendarEventBot.DAL.Persistence.FileProvider;

namespace TeamCalendarEventBot.DAL.Persistence
{
    public class UserBotDao : IUserBotDao
    {
        private const string _fileName = "users.dat";
        private static object _locker = new object();
        private readonly IFileProvider _fileProvider;

        public UserBotDao(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public IEnumerable<UserBotData> GetAllUsers()
        {
            lock (_locker)
            {
                var dataContents = _fileProvider.ReadFile(_fileName);
                if (string.IsNullOrWhiteSpace(dataContents))
                {
                    return new List<UserBotData>();
                }

                return JsonConvert.DeserializeObject<List<UserBotData>>(dataContents) ?? new List<UserBotData>();
            }
        }

        public void UpsertUser(UserBotData userBot)
        {
            lock (_locker)
            {
                var all = GetAllUsers().ToList();
                var exist = all.FirstOrDefault(x => x.ChatId == userBot.ChatId);
                if (exist != null)
                {
                    all.Remove(exist);
                }
                all.Add(userBot);

                all = all.OrderBy(x => x.Username).ToList();
                var newDataContents = JsonConvert.SerializeObject(all);
                _fileProvider.WriteFile(_fileName, newDataContents);
            }
        }
    }
}
