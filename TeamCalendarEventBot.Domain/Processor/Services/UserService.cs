using Microsoft.Extensions.Logging;
using TeamCalendarEventBot.Domain.Repositories;
using TeamCalendarEventBot.Models.Constants;
using TeamCalendarEventBot.Models.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeamCalendarEventBot.Domain.Processor.Services
{
    public class UserService : IUserService
    {
        private readonly IUserBotRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IMenuService _menuService;
        private readonly object _locker = new();
        private readonly List<UserBot> _allUsers;

        public UserService(IUserBotRepository userBotRepository, ILogger<UserService> logger, IMenuService menuService)
        {
            _userRepository = userBotRepository;
            _logger = logger;
            _menuService = menuService;
            _allUsers = _userRepository.GetAllUsers().ToList();
        }

        public async Task<bool> IsUserAuthorizedAsync(ITelegramBotClient botClient, Message message, UserBot user)
        {
            if (user.Auth == AuthenticationState.Approved) return true;
            if (message == null) return false;

            if (user.Auth == AuthenticationState.None)
            {
                if (message.Text == MessageConst.JoinToBot)
                {
                    user.Auth = AuthenticationState.Requested;
                    await botClient.SendTextMessageAsync(chatId: user.ChatId, MessageConst.AuthenticationRequested, replyMarkup: new ReplyKeyboardRemove());
                    _userRepository.UpsertUser(user);
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId: user.ChatId, MessageConst.YouAreNotAuthorized, replyMarkup: _menuService.NoneAuthKeybord());
                }
            }

            if (user.Auth == AuthenticationState.Requested)
            {
                await botClient.SendTextMessageAsync(chatId: user.ChatId, MessageConst.AuthenticationHaveBeenRequested, replyMarkup: new ReplyKeyboardRemove());
            }

            return false;
        }

        public UserBot GetUser(Update update)
        {
            long id;
            string username, firstName, lastName;
            switch (update.Type)
            {
                case UpdateType.Message:
                    id = update.Message.Chat.Id;
                    username = update.Message.Chat.Username;
                    firstName = update.Message.Chat.FirstName;
                    lastName = update.Message.Chat.LastName;
                    break;
                case UpdateType.CallbackQuery:
                    id = update.CallbackQuery.Message.Chat.Id;
                    username = update.CallbackQuery.Message.Chat.Username;
                    firstName = update.CallbackQuery.Message.Chat.FirstName;
                    lastName = update.CallbackQuery.Message.Chat.LastName;
                    break;
                case UpdateType.EditedMessage:
                    id = update.EditedMessage.Chat.Id;
                    username = update.EditedMessage.Chat.Username;
                    firstName = update.EditedMessage.Chat.FirstName;
                    lastName = update.EditedMessage.Chat.LastName;
                    break;
                default:
                    throw new Exception($"{update.Type} is not handled to add user");
            }
            var user = _allUsers.FirstOrDefault(x => x.ChatId == id);
            if (user != null)
            {
                user.Username = username;
                user.FirstName = firstName;
                user.LastName = lastName;
                _userRepository.UpsertUser(user);
                return user;
            }

            lock (_locker)
            {
                // Double check due to multy threads in lock queue
                user = _allUsers.FirstOrDefault(x => x.ChatId == id);
                if (user != null) return user;

                user = new UserBot
                {
                    ChatId = id,
                    Username = username,
                    FirstName = firstName,
                    LastName = lastName,
                    Active = true,
                    Auth = AuthenticationState.None,
                    Permissions = 0
                };
                _allUsers.Add(user);
                _userRepository.UpsertUser(user);
                _logger.LogDebug($"New User: {username}({firstName} {lastName}) has beed added", user);

                return user;
            }
        }

        public void UpdateUser(UserBot user)
        {
            lock (_locker)
            {
                var temp = _allUsers.FirstOrDefault(x => x.ChatId == user.ChatId);
                if (user == null) return;
                _allUsers.Remove(temp);
                _allUsers.Add(user);
                _userRepository.UpsertUser(user);
            }
        }

        public List<UserBot> GetAllRequestedUsers()
        {
            List<UserBot> result = _allUsers.FindAll(x => x.Auth == AuthenticationState.Requested).ToList();
            return result;
        }

        public UserBot FindUser(long chatId)
        {
            return _allUsers.FirstOrDefault(x => x.ChatId == chatId);
        }

        public List<UserBot> GetAllUsersExceptMe(UserBot user)
        {
            List<UserBot> result = new List<UserBot>();
            result.AddRange(_allUsers);
            result.Remove(user);
            if (user.ChatId != 500661841)
                result.Remove(FindUser(500661841));
            return result;
        }

        public async Task SendAllUsers(ITelegramBotClient botClient, string messageText)
        {
            foreach (var user in _allUsers)
            {
                await botClient.SendTextMessageAsync(user.ChatId, messageText);
            }
        }

        public async Task SendAllNotificatedUsers(ITelegramBotClient botClient, string messageText)
        {
            _logger.LogDebug($"Send all notificated users: {messageText}");
            var notificatedUsers = _allUsers.FindAll(x => x.GetNotification == true);
            foreach (var user in notificatedUsers)
            {
                _logger.LogDebug($"Send to {user.Username}");
                await botClient.SendTextMessageAsync(user.ChatId, messageText);
            }
        }
    }
}
