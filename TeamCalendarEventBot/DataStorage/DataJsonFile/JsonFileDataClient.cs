namespace TeamCalendarEventBot.DataStorage.DataJsonFile
{
    public class JsonFileDataClient : IDataClient
    {
        private IUserInfoDataProvider _userInfoDataProvider;
        private IEventDataProvider _eventDataProvider;
        public IUserInfoDataProvider UserInfoDataProvider => _userInfoDataProvider ??= new UserInfoDataProvider(new FileProvider());
        public IEventDataProvider EventDataProvider => _eventDataProvider ??= new EventDataProvider(new FileProvider());
    }
}
