namespace TeamCalendarEventBot.DataStorage.DataJsonFile
{
    public class JsonFileDataClient : IDataClient
    {
        private IUserInfoDataProvider _userInfoDataProvider;
        public IUserInfoDataProvider UserInfoDataProvider => _userInfoDataProvider ??= new UserInfoDataProvider(new FileProvider());
    }
}
