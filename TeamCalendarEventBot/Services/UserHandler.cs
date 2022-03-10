﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamCalendarEventBot.Constants;
using TeamCalendarEventBot.DataStorage;
using TeamCalendarEventBot.DataStorage.DataJsonFile;
using TeamCalendarEventBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeamCalendarEventBot.Services
{
    public static class UserHandler
    {
        private static readonly IUserInfoDataProvider _dataProvider;
        private static readonly object _locker = new();
        private static readonly List<UserBot> _allUsers;

        static UserHandler()
        {
            _dataProvider = new JsonFileDataClient().UserInfoDataProvider;
            _allUsers = _dataProvider.GetAllUsers();
        }

        public static async Task<bool> IsUserAuthorizedAsync(ITelegramBotClient botClient, Message message, UserBot user)
        {
            if (user.Auth == AuthenticationState.Approved) return true;
            if (message == null) return false;

            if (user.Auth == AuthenticationState.None)
            {
                if (message.Text == MessageConst.JoinToBot)
                {
                    user.Auth = AuthenticationState.Requested;
                    _dataProvider.UpsertUser(user);
                }
                else
                {
                    _ = await botClient.SendTextMessageAsync(chatId: user.ChatId, "Вы не авторизированы", replyMarkup: Menu.NoneAuthKeybord());
                }
            }

            if (user.Auth == AuthenticationState.Requested)
            {
                _ = await botClient.SendTextMessageAsync(chatId: user.ChatId, "Авторизация запрошена", replyMarkup: Menu.NoneAuthKeybord());
            }

            return false;
        }

        public static UserBot GetUser(Update update)
        {
            long id;
            string username;
            switch (update.Type)
            {
                case UpdateType.Message:
                    id = update.Message.Chat.Id;
                    username = update.Message.Chat.Username;
                    break;
                case UpdateType.CallbackQuery:
                    id = update.CallbackQuery.Message.Chat.Id;
                    username = update.CallbackQuery.Message.Chat.Username;
                    break;
                case UpdateType.EditedMessage:
                    id = update.EditedMessage.Chat.Id;
                    username = update.EditedMessage.Chat.Username;
                    break;
                default:
                    Console.WriteLine(update.Type + " is not handled");
                    id = 0;
                    username = "unknown";
                    break;
            }
            
            var user = _allUsers.FirstOrDefault(x => x.ChatId == id);
            if (user != null) return user;

            lock (_locker)
            {
                // Double check due to multy threads in lock queue
                user = _allUsers.FirstOrDefault(x => x.ChatId == id);
                if (user != null) return user;

                user = new UserBot
                {
                    ChatId = id,
                    Username = username,
                    Active = true,
                    Auth = AuthenticationState.None,
                    Permissions = 0
                };
                _allUsers.Add(user);
                _dataProvider.UpsertUser(user);
                Console.WriteLine($"New User: {id} has beed added");

                return user;
            }
        }
    }
}
