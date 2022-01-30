namespace TeamCalendarEventBot.DataStorage.DataJsonFile
{
    public abstract class JsonFileDataProviderBase
    {
        protected IFileProvider FileProvider { get; }

        protected JsonFileDataProviderBase(IFileProvider fileProvider)
        {
            FileProvider = fileProvider;
        }
    }
}
