namespace TeamCalendarEventBot.DAL.Persistence.FileProvider
{
    public interface IFileProvider
    {
        string ReadFile(string fileName);

        bool WriteFile(string path, string text);
    }
}
