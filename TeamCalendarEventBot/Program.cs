using System;
using TeamCalendarEventBot.DataStorage;
using TeamCalendarEventBot.DataStorage.DataJsonFile;
using TeamCalendarEventBot.Models;

namespace TeamCalendarEventBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IDataClient dataClient = new JsonFileDataClient();

            var allUser = dataClient.UserInfoDataProvider.GetAllUsers();


            Console.ReadLine();
        }
    }
}
