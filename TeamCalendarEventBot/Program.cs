using System;
using TeamCalendarEventBot.DataStorage;
using TeamCalendarEventBot.DataStorage.DataJsonFile;
using TeamCalendarEventBot.Models;
using Telegram.Bot;
using TeamCalendarEventBot.Constants;
using TeamCalendarEventBot.Sevices;

namespace TeamCalendarEventBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TelegramBotClient client = new TelegramBotClient(TelegramBotInfo.token);

            Updates updates = new Updates(client);
            updates.ProcessUpdates();

            Console.ReadLine();
        }
    }
}
