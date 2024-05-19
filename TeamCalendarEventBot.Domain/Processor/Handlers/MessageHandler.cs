using Microsoft.Extensions.Logging;
using TeamCalendarEventBot.Domain.Processor.Services;
using TeamCalendarEventBot.Models.Constants;
using TeamCalendarEventBot.Models.Models;
using TeamCalendarEventBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeamCalendarEventBot.Domain.Processor.Handlers
{
    public class MessageHandler
    {
        private readonly UserService _userService;
        private readonly CalendarService _calendarService;
        private readonly MenuService _menuService;
        private readonly EventService _eventService;
        private readonly NotificationService _notificationService;
        private readonly ILogger<MessageHandler> _logger;

        public MessageHandler(
            UserService userService,
            ILogger<MessageHandler> logger,
            CalendarService calendarService,
            MenuService menuService,
            EventService eventService,
            NotificationService notificationService)
        {
            _userService = userService;
            _logger = logger;
            _calendarService = calendarService;
            _menuService = menuService;
            _eventService = eventService;
            _notificationService = notificationService;
        }

        public async Task BotOnMessageReceivedAsync(ITelegramBotClient botClient, Message message, UserBot user)
        {
            if (message.Type != MessageType.Text) return;

            if (user.UserStatus != UserStatus.None)
            {
                switch (user.UserStatus)
                {
                    case UserStatus.Adding:
                        await OnAddingTextToEventMessageAsync(botClient, user, message.Text);
                        break;
                    default:
                        break;
                };
                return;
            }
            var action = message.Text! switch
            {
                //Commands
                MessageConst.StartCommand => StartupMessageAsync(botClient, user),
                MessageConst.RunNotifications => RunNotificationsCommand(botClient, user),
                //Startapmenu
                MessageConst.Calendar => CalendarMessageAsync(botClient, user),
                MessageConst.WatchTimetable => WatchTimetableMessageAsync(botClient, user),
                MessageConst.CheckAuthenticationRequests => AuthenticationMessageAsync(botClient, user),
                MessageConst.ManagePermissions => ManagePermissionsMessageAsync(botClient, user),
                MessageConst.GettingNotifications => GettingNotificationsMessageAsync(botClient, user),
                MessageConst.Info => InfoMessageAsync(botClient, user),
                //CalendarMenu
                MessageConst.AddEvent => AddEventMessageAsync(botClient, user),
                MessageConst.OnWeekEvents => OnWeekEventsMessageAsync(botClient, user),
                MessageConst.EditEvents => EditEventsMessageAsync(botClient, user),
                //General
                MessageConst.BackToMainMenu => StartupMessageAsync(botClient, user),
                _ => UnknownMessageAsync(botClient, user)
            };

            try
            {
                await action;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "OnMessage error", user);
            }
        }

        private async Task CalendarMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            if (((Permission)user.Permissions & Permission.View) != Permission.View)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.Calendar, replyMarkup: _calendarService.GetCalendarKeyboard(DateTime.Today, CallbackConst.GetEvents));
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.ChoseAction, replyMarkup: _menuService.GetMenuButtons((Permission)user.Permissions, MenuStage.CalendarMenu));
        }

        private async Task WatchTimetableMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            await botClient.SendTextMessageAsync(user.ChatId, "https://cist.nure.ua/ias/app/tt/f?p=778:201:3968484794665844:::201:P201_FIRST_DATE,P201_LAST_DATE,P201_GROUP,P201_POTOK:01.09.2022,31.01.2023,9287984,0");
        }

        private async Task StartupMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.ChoseAction, replyMarkup: _menuService.GetMenuButtons((Permission)user.Permissions, MenuStage.MainMenu));
        }

        private async Task UnknownMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            _logger.LogDebug("Unknown message", user);
            await botClient.SendTextMessageAsync(user.ChatId, text: MessageConst.UnknownMessage);
        }

        private async Task AddEventMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.Calendar, replyMarkup: _calendarService.GetCalendarKeyboard(DateTime.Today, CallbackConst.Adding));
        }

        private async Task OnWeekEventsMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            if (((Permission)user.Permissions & Permission.View) != Permission.View)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            await _eventService.ShowCalendarEventsByWeekAsync(botClient, DateTime.Today, user);
        }

        private async Task OnAddingTextToEventMessageAsync(ITelegramBotClient botClient, UserBot user, string message)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            CalendarEvent tempEvent = new CalendarEvent() { Date = user.TempDate, Text = message };
            _eventService.AddGeneralEventAsync(tempEvent);
            List<List<InlineKeyboardButton>> keyboardButtons = new List<List<InlineKeyboardButton>> {
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton(MessageConst.EventTypeDefault) { CallbackData = $"{CallbackConst.AddEventType} {(int)CalendarEventType.Default} {tempEvent.Id}"}, new InlineKeyboardButton(MessageConst.EventTypeDeadline) { CallbackData = $"{CallbackConst.AddEventType} {(int)CalendarEventType.Deadline} {tempEvent.Id}"} },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton(MessageConst.EventTypeBirthday) { CallbackData = $"{CallbackConst.AddEventType} {(int)CalendarEventType.Birthday} {tempEvent.Id}"} },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton(MessageConst.CancelAdding) { CallbackData = $"{CallbackConst.CancelAdding} {tempEvent.Id}" } }
            };
            await botClient.SendTextMessageAsync(user.ChatId, $"{tempEvent.Date.ToString("dd.MM.yyyy")}\n{tempEvent.Text}\n\n{MessageConst.ChoseEventType}:", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
            user.UserStatus = UserStatus.None;
            _userService.UpdateUser(user);
        }
        private async Task AuthenticationMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            if (((Permission)user.Permissions & Permission.Authorizating) != Permission.Authorizating)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            List<UserBot> users = _userService.GetAllRequestedUsers();
            if (users.Count == 0)
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NoAuthenticationRequests);
            foreach (var item in users)
            {
                List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton> {
                    new InlineKeyboardButton("Авторизувати") { CallbackData = $"{CallbackConst.Authentication} {item.ChatId}"} };
                await botClient.SendTextMessageAsync(user.ChatId, $"@{item.Username} ({item.FirstName} {item.LastName})", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
            }
        }
        private async Task ManagePermissionsMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            if (((Permission)user.Permissions & Permission.GivingPermissions) != Permission.GivingPermissions)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            List<UserBot> users = _userService.GetAllUsersExceptMe(user);
            if (users.Count == 0)
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NoUsersExist);
            foreach (UserBot item in users)
            {
                List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton> {
                    new InlineKeyboardButton(MessageConst.ChangePermissions) { CallbackData = $"{CallbackConst.ManagePermissions} {item.ChatId}"} };
                await botClient.SendTextMessageAsync(user.ChatId, $"@{item.Username} ({item.FirstName} {item.LastName})", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
            }
        }

        private async Task EditEventsMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.Calendar, replyMarkup: _calendarService.GetCalendarKeyboard(DateTime.Today, CallbackConst.EditEvent));
        }

        private Task RunNotificationsCommand(ITelegramBotClient botClient, UserBot user)
        {
            _notificationService.StartNotifications(botClient);
            _logger.LogDebug("RunNotificationsCommand", user);
            return Task.CompletedTask;
        }

        private async Task GettingNotificationsMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            if (((Permission)user.Permissions & Permission.View) != Permission.View)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton> {
                    new InlineKeyboardButton(MessageConst.Yes) { CallbackData = $"{CallbackConst.ChangeMyNotificationStatus} {1} {user.ChatId}"},
                    new InlineKeyboardButton(MessageConst.No) { CallbackData = $"{CallbackConst.ChangeMyNotificationStatus} {0} {user.ChatId}"}};
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.DoesGetNotifications, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
        }

        private async Task InfoMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.InfoAboutBot);
        }
    }
}
