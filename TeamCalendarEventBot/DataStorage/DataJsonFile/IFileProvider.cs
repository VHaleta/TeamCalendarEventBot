namespace TeamCalendarEventBot.DataStorage.DataJsonFile
{
    public interface IFileProvider
    {
        string ReadFile(string fileName);
        bool WriteFile(string path, string text);
    }
}
