namespace TeamCalendarEventBot.DataStorage
{
    public interface IDataClient
    {
        IUserInfoDataProvider UserInfoDataProvider { get; }
    }
}
