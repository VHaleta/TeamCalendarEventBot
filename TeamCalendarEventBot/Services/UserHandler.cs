using System;
using System.Collections.Generic;
using System.Linq;
using TeamCalendarEventBot.Constants;
using TeamCalendarEventBot.DataStorage;
using TeamCalendarEventBot.DataStorage.DataJsonFile;
using TeamCalendarEventBot.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeamCalendarEventBot.Services
{
    public static class UserHandler
    {
        private static readonly IUserInfoDataProvider _dataProvider;
        private static readonly Object _lockObj = new();
        private static readonly List<UserBot> _allUsers;

        static UserHandler()
        {
            _dataProvider = new JsonFileDataClient().UserInfoDataProvider;
            _allUsers = _dataProvider.GetAllUsers();
        }

        public static UserBot GetUser(Update update)
        {
            long id = update.Type switch
            {
                UpdateType.Message => update.Message.Chat.Id,
                UpdateType.CallbackQuery => update.CallbackQuery.Message.Chat.Id,
                UpdateType.EditedMessage => update.EditedMessage.Chat.Id,
                _ => 0
            };

            if (id == 0)
            {
                Console.WriteLine(update.Type + " is not handled");
            }

            lock (_lockObj)
            {
                var userBot = _allUsers.FirstOrDefault(x => x.ChatId == id);
                if (userBot == null)
                {
                    Console.WriteLine($"New User: {id} has beed added");
                    userBot = new UserBot
                    {
                        ChatId = id,
                        Active = true,
                        Auth = AuthenticationState.None,
                        Permissions = 0
                    };
                    _allUsers.Add(userBot);
                    _dataProvider.UpsertUser(userBot);
                }
                return userBot;
          }
        }
    }
}
