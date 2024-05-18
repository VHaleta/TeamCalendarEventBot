namespace TeamCalendarEventBot.DAL.Persistence.FileProvider
{
    public class FileProvider : IFileProvider
    {
        protected string _directoryPath = $"{AppContext.BaseDirectory}Data";

        public FileProvider()
        {
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
        }

        private string GetFullPath(string fileName) => $"{_directoryPath}/{fileName}";

        public string ReadFile(string fileName)
        {
            try
            {
                return File.ReadAllText(GetFullPath(fileName));
            }
            catch (Exception)
            {
                // TODO: Log error
                return null;
            }
        }

        public bool WriteFile(string fileName, string contents)
        {
            try
            {
                File.WriteAllText(GetFullPath(fileName), contents);

                return true;
            }
            catch (Exception)
            {
                // TODO: Log error
                return false;
            }
        }
    }
}
