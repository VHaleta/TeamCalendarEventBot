using System;
using System.IO;

namespace TeamCalendarEventBot.DataStorage.DataJsonFile
{
    public class FileProvider : IFileProvider
    {
        #region Fields
        protected string _directoryPath = $"{System.AppContext.BaseDirectory}Data";
        #endregion

        #region Constructors
        public FileProvider()
        {
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
        }
        #endregion

        #region Protected Methods
        protected string GetFullPath(string fileName) => $"{_directoryPath}\\{fileName}";
        #endregion

        #region Public Methods
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
        #endregion
    }
}
