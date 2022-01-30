namespace TeamCalendarEventBot.DataStorage.DataJsonFile
{
    public interface IFileProvider
    {
        string ReadFile(string path);
        bool WriteFile(string path, string text);
    }
}
