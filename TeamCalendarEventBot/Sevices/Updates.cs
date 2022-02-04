using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TeamCalendarEventBot.Constants;
using TeamCalendarEventBot.DataStorage;
using TeamCalendarEventBot.DataStorage.DataJsonFile;
using TeamCalendarEventBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeamCalendarEventBot.Sevices
{
    public class Updates
    {
        private TelegramBotClient client;
        IDataClient dataClient;
        List<UserBot> allUsers;
        public Updates(TelegramBotClient client)
        {
            this.client = client;
            dataClient = new JsonFileDataClient();
            allUsers = dataClient.UserInfoDataProvider.GetAllUsers();
        }
        public void ProcessUpdates()
        {
            int offset = 0;
            while (true)
            {
                var updates = client.GetUpdatesAsync(offset).Result;
                if (updates != null && updates.Count() > 0)
                {
                    allUsers = dataClient.UserInfoDataProvider.GetAllUsers();
                    foreach (var update in updates)
                    {
                        ProcessUpdate(update);
                        offset = update.Id + 1;
                    }
                }
                Thread.Sleep(500);
            }
        }

        private void ProcessUpdate(Update update)
        {
            UserCheck(update);
        }

        private void UserCheck(Update update)
        {
            long id = 0;
            switch (update.Type)
            {
                case UpdateType.Message:
                    id = update.Message.Chat.Id;
                    break;
                case UpdateType.CallbackQuery:
                    id = update.CallbackQuery.Message.Chat.Id;
                    break;
                case UpdateType.EditedMessage:
                    id = update.EditedMessage.Chat.Id;
                    break;
                default:
                    Console.WriteLine(update.Type + " not implemented");
                    break;
            }
            var userBot = allUsers.FirstOrDefault(x => x.ChatId == id);
            if (userBot == null)
            {
                Console.WriteLine(id + " new user");
                dataClient.UserInfoDataProvider.UpsertUser(new UserBot { ChatId = id, Active = false, Auth = AuthenticationState.None, Permissions = 0 });
            }
            else ShowUser(id);
        }
        
        private void ShowUser(long id) //temp
        {
            var userBot = allUsers.FirstOrDefault(x => x.ChatId == id);
            Console.WriteLine($"\nUser\nChat id {userBot.ChatId}\nActive {userBot.Active}\nPremissions {userBot.Permissions}\nActive {userBot.Active}\nAuth {userBot.Auth}");
        }
    }
}
