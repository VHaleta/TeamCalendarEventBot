using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamCalendarEventBot.Constants;
using TeamCalendarEventBot.DataStorage;
using TeamCalendarEventBot.DataStorage.DataJsonFile;
using TeamCalendarEventBot.Logger;
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
                    await botClient.SendTextMessageAsync(chatId: user.ChatId, MessageConst.AuthenticationRequested, replyMarkup: Menu.NoneAuthKeybord());
                    _dataProvider.UpsertUser(user);
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId: user.ChatId, MessageConst.YouAreNotAuthorized, replyMarkup: Menu.NoneAuthKeybord());
                }
            }

            if (user.Auth == AuthenticationState.Requested)
            {
                await botClient.SendTextMessageAsync(chatId: user.ChatId, MessageConst.AuthenticationHaveBeenRequested, replyMarkup: Menu.NoneAuthKeybord());
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
                    throw new Exception($"{update.Type} is not handled to add user");
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
                LogHandler.LogDebug($"New User: {id} has beed added", user);

                return user;
            }
        }

        public static void UpdateUser(UserBot user)
        {
            lock (_locker)
            {
                var temp = _allUsers.FirstOrDefault(x => x.ChatId == user.ChatId);
                if (user == null) return;
                _allUsers.Remove(temp);
                _allUsers.Add(user);
                _dataProvider.UpsertUser(user);
            }
        }

        public static List<UserBot> GetAllRequestedUsers()
        {
            List<UserBot> result = _allUsers.FindAll(x => x.Auth == AuthenticationState.Requested).ToList();
            return result;
        }

        public static UserBot FindUser(long chatId)
        {
            return _allUsers.FirstOrDefault(x => x.ChatId == chatId);
        }

        public static List<UserBot> GetAllUsersExceptMe(UserBot user)
        {
            List<UserBot> result = new List<UserBot>();
            result.AddRange(_allUsers);
            result.Remove(user);
            return result;
        }

        public static async Task SendAllUsers(ITelegramBotClient botClient, string messageText)
        {
            foreach (var user in _allUsers)
            {
                await botClient.SendTextMessageAsync(user.ChatId, messageText);
            }
        }

        public static async Task SendAllNotificatedUsers(ITelegramBotClient botClient, string messageText)
        {
            var notificatedUsers = _allUsers.FindAll(x => x.GetNotification == true);
            foreach (var user in notificatedUsers)
            {
                await botClient.SendTextMessageAsync(user.ChatId, messageText);
            }
        } 
    }
}
