﻿using Microsoft.Extensions.Logging;
using TeamCalendarEventBot.Domain.Helpers;
using TeamCalendarEventBot.Domain.Processor.Services;
using TeamCalendarEventBot.Models.Constants;
using TeamCalendarEventBot.Models.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeamCalendarEventBot.Domain.Processor.Handlers
{
    public class CallbackQueryHandler : ICallbackQueryHandler
    {
        private readonly IUserService _userService;
        private readonly ICalendarService _calendarService;
        private readonly IEventService _eventService;
        private readonly ILogger<CallbackQueryHandler> _logger;

        public CallbackQueryHandler(
            IUserService userService,
            ILogger<CallbackQueryHandler> logger,
            ICalendarService calendarService,
            IEventService eventService)
        {
            _userService = userService;
            _logger = logger;
            _calendarService = calendarService;
            _eventService = eventService;
        }

        public async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user)
        {
            user.UserStatus = UserStatus.None;
            string data = callbackQuery.Data;
            string[] dataSplit = data.Split();
            var action = dataSplit[0]! switch
            {
                CallbackConst.ChangeMonth => ChangeMonthCallbackQueryAsync(botClient, callbackQuery, user, dataSplit),
                CallbackConst.GetEvents => ShowCalendarEventsByDateCallbackQueryAsync(botClient, user, dataSplit),
                CallbackConst.Adding => AddingEventCallbackQueryAsync(botClient, callbackQuery, user, dataSplit),
                CallbackConst.Authentication => AuthenticateUserCallbackQueryAsync(botClient, user, dataSplit),
                CallbackConst.ManagePermissions => ManagePermissionsCallbackQueryAsync(botClient, callbackQuery, user, dataSplit),
                CallbackConst.ChangePermission => ChangePermissionCallbackQueryAsync(botClient, callbackQuery, user, dataSplit),
                CallbackConst.DeleteEvent => DeleteEventCallbackQueryAsync(botClient, callbackQuery, user, dataSplit),
                CallbackConst.EditEvent => EditEventsCallbackQueryAsync(botClient, user, dataSplit),
                CallbackConst.AddEventType => AddEventTypeCallbackQueryAsync(botClient, callbackQuery, user, dataSplit),
                CallbackConst.ChangeNotification => ChangeNotificationCallbackQueryAsync(botClient, callbackQuery, user, dataSplit),
                CallbackConst.AddEventToCommonCalendar => AddEventToCommonCalendarAsync(botClient, callbackQuery, user, dataSplit),
                CallbackConst.CancelAdding => CancelAddingCallbackQueryAsync(botClient, callbackQuery, user, dataSplit),
                CallbackConst.GoToFinalAddingStage => GoToFinalAddingStageCallbackQueryAsync(botClient, callbackQuery, user, dataSplit),
                CallbackConst.ChangeMyNotificationStatus => ChangeMyNotificationStatusCallbackQueryAsync(botClient, user, dataSplit),
                _ => UnknownCallbackQuery(botClient, user)
            };
            try
            {
                await action;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "OnCallbackQuery error", user);
            }
        }

        private async Task ChangeMonthCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            int month = 0, year = 0;
            if (!int.TryParse(dataSplit[2], out month) || !int.TryParse(dataSplit[3], out year))
                throw new Exception($"Wrong format of month or year: {dataSplit[2]} {dataSplit[3]}");
            DateTime date = new DateTime(year, month, 1);
            await botClient.EditMessageReplyMarkupAsync(chatId: user.ChatId, callbackQuery.Message.MessageId, replyMarkup: _calendarService.GetCalendarKeyboard(date, dataSplit[1]));
        }

        private async Task ShowCalendarEventsByDateCallbackQueryAsync(ITelegramBotClient botClient, UserBot user, string[] dataSplit)
        {
            var date = dataSplit[1].CustomDateParse();
            await _eventService.ShowCalendarEventsByDateAsync(botClient, date, user);
        }

        private async Task InfoMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.InfoAboutBot);
        }

        private async Task AddingEventCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            var date = dataSplit[1].CustomDateParse();
            user.UserStatus = UserStatus.Adding;
            user.TempDate = date;
            List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton>();
            await botClient.EditMessageTextAsync(user.ChatId, callbackQuery.Message.MessageId, $"{MessageConst.YouHaveChosenDate}: {date.ToString("dd.MM.yyyy")}\n{MessageConst.WriteEventText}:", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
            _userService.UpdateUser(user);
        }

        private async Task AuthenticateUserCallbackQueryAsync(ITelegramBotClient botClient, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.Authorizating) != Permission.Authorizating)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            long chatId;
            if (!long.TryParse(dataSplit[1], out chatId))
                throw new Exception($"Wrong format of chatId: {dataSplit[1]}");

            UserBot authUser = _userService.FindUser(chatId);
            if (authUser == null)
                throw new Exception($"User {chatId} doesn`t exist");

            authUser.Auth = AuthenticationState.Approved;
            _userService.UpdateUser(authUser);
            await botClient.SendTextMessageAsync(authUser.ChatId, MessageConst.YouHaveBeenAuthorized);
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.UserHaveBeenAuthorized(authUser.Username));
        }
        private async Task ManagePermissionsCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.GivingPermissions) != Permission.GivingPermissions)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            long chatId;
            if (!long.TryParse(dataSplit[1], out chatId))
                throw new Exception($"Wrong format of chatId: {dataSplit[1]}");

            UserBot managedUser = _userService.FindUser(chatId);
            if (managedUser == null)
                throw new Exception($"User {chatId} doesn`t exist");

            List<List<InlineKeyboardButton>> keyboardButtons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionView} - {(((Permission)managedUser.Permissions & Permission.View) == Permission.View ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.View} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionOwnCalendar} - {(((Permission)managedUser.Permissions & Permission.OwnCalendar) == Permission.OwnCalendar ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.OwnCalendar} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionCommonCalendar} - {(((Permission)managedUser.Permissions & Permission.CommonCalendar) == Permission.CommonCalendar ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.CommonCalendar} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionAuthorizating} - {(((Permission)managedUser.Permissions & Permission.Authorizating) == Permission.Authorizating ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.Authorizating} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionGivingPermissions} - {(((Permission)managedUser.Permissions & Permission.GivingPermissions) == Permission.GivingPermissions ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.GivingPermissions} {managedUser.ChatId}" } }
            };
            await botClient.EditMessageReplyMarkupAsync(chatId: user.ChatId, callbackQuery.Message.MessageId, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));

        }
        private async Task ChangePermissionCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.GivingPermissions) != Permission.GivingPermissions)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            long chatId;
            if (!long.TryParse(dataSplit[2], out chatId))
                throw new Exception($"Wrong format of chatId: {dataSplit[2]}");

            int permission;
            if (!int.TryParse(dataSplit[1], out permission))
                throw new Exception($"Wrong format of permission: {dataSplit[1]}");

            UserBot managedUser = _userService.FindUser(chatId);
            if (managedUser == null) throw new Exception($"Null user {chatId}");
            Permission permissions = (Permission)managedUser.Permissions;
            switch ((Permission)permission)
            {
                case Permission.View:
                    if ((permissions & Permission.View) == Permission.View)
                        permissions &= ~Permission.View;
                    else
                        permissions |= Permission.View;
                    break;
                case Permission.OwnCalendar:
                    if ((permissions & Permission.OwnCalendar) == Permission.OwnCalendar)
                        permissions &= ~Permission.OwnCalendar;
                    else
                        permissions |= Permission.OwnCalendar;
                    break;
                case Permission.CommonCalendar:
                    if ((permissions & Permission.CommonCalendar) == Permission.CommonCalendar)
                        permissions &= ~Permission.CommonCalendar;
                    else
                        permissions |= Permission.CommonCalendar;
                    break;
                case Permission.Authorizating:
                    if ((permissions & Permission.Authorizating) == Permission.Authorizating)
                        permissions &= ~Permission.Authorizating;
                    else
                        permissions |= Permission.Authorizating;
                    break;
                case Permission.GivingPermissions:
                    if ((permissions & Permission.GivingPermissions) == Permission.GivingPermissions)
                        permissions &= ~Permission.GivingPermissions;
                    else
                        permissions |= Permission.GivingPermissions;
                    break;
                default:
                    break;
            }
            managedUser.Permissions = (int)permissions;
            _userService.UpdateUser(managedUser);
            List<List<InlineKeyboardButton>> keyboardButtons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionView} - {(((Permission)managedUser.Permissions & Permission.View) == Permission.View ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.View} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionOwnCalendar} - {(((Permission)managedUser.Permissions & Permission.OwnCalendar) == Permission.OwnCalendar ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.OwnCalendar} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionCommonCalendar} - {(((Permission)managedUser.Permissions & Permission.CommonCalendar) == Permission.CommonCalendar ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.CommonCalendar} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionAuthorizating} - {(((Permission)managedUser.Permissions & Permission.Authorizating) == Permission.Authorizating ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.Authorizating} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionGivingPermissions} - {(((Permission)managedUser.Permissions & Permission.GivingPermissions) == Permission.GivingPermissions ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.GivingPermissions} {managedUser.ChatId}" } }
            };
            await botClient.EditMessageReplyMarkupAsync(user.ChatId, callbackQuery.Message.MessageId, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));

        }

        private async Task DeleteEventCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            Guid Id;
            if (!Guid.TryParse(dataSplit[1], out Id))
                throw new Exception($"Wrong format of event id: {dataSplit[1]}");

            _eventService.DeleteGeneralEvent(Id);
            List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton>();
            await botClient.EditMessageTextAsync(user.ChatId, callbackQuery.Message.MessageId, MessageConst.EventHasBeenDeleted, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));

        }

        private async Task EditEventsCallbackQueryAsync(ITelegramBotClient botClient, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            var date = dataSplit[1].CustomDateParse();
            await _eventService.EditCalendarEventsByDateAsync(botClient, date, user);
        }

        private Task UnknownCallbackQuery(ITelegramBotClient botClient, UserBot user)
        {
            _logger.LogDebug("Unknown callback query", user);
            return Task.CompletedTask;
        }

        private async Task AddEventTypeCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            int type;
            if (!int.TryParse(dataSplit[1], out type))
                throw new Exception($"Wrong format of event type: {dataSplit[1]}");

            Guid eventId;
            if (!Guid.TryParse(dataSplit[2], out eventId))
                throw new Exception($"Wrong format of event id: {dataSplit[2]}");

            CalendarEvent tempEvent = _eventService.FindEvent(eventId);
            if (tempEvent == null)
                throw new Exception($"Event {eventId} doesn`t exist");

            tempEvent.Type = (CalendarEventType)type;
            _eventService.EditEvent(tempEvent);
            List<List<InlineKeyboardButton>> keyboardButtons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.NotificationForOneDay} - {MessageConst.No}") { CallbackData = $"{CallbackConst.ChangeNotification} {(int)Notification.ForOneDay} {tempEvent.Id}" },
                                                    new InlineKeyboardButton($"{MessageConst.NotificationForTwoDays} - {MessageConst.No}") { CallbackData = $"{CallbackConst.ChangeNotification} {(int)Notification.ForTwoDays} {tempEvent.Id}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.NotificationForAWeek} - {MessageConst.No}") { CallbackData = $"{CallbackConst.ChangeNotification} {(int)Notification.ForAWeek} {tempEvent.Id}" },
                                                    new InlineKeyboardButton($"{MessageConst.NotificationInDay} - {MessageConst.No}") { CallbackData = $"{CallbackConst.ChangeNotification} {(int)Notification.InDay} {tempEvent.Id}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton(MessageConst.Next) {CallbackData = $"{CallbackConst.GoToFinalAddingStage} {tempEvent.Id}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton(MessageConst.CancelAdding) { CallbackData = $"{CallbackConst.CancelAdding} {tempEvent.Id}" } }
            };
            await botClient.EditMessageTextAsync(user.ChatId, callbackQuery.Message.MessageId, $"{tempEvent.Date.ToString("dd.MM.yyyy")}\n{tempEvent.Text}\n{MessageConst.EventType}: {tempEvent.Type}\n\n{MessageConst.ChoseNotification}:", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
        }
        private async Task ChangeNotificationCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            int notification;
            if (!int.TryParse(dataSplit[1], out notification))
                throw new Exception($"Wrong format of notification: {dataSplit[1]}");

            Guid eventId;
            if (!Guid.TryParse(dataSplit[2], out eventId))
                throw new Exception($"Wrong format of event id: {dataSplit[2]}");

            CalendarEvent tempEvent = _eventService.FindEvent(eventId);
            if (tempEvent == null)
                throw new Exception($"Event {eventId} doesn`t exist");

            Notification notifications = (Notification)tempEvent.Notifications;
            switch ((Notification)notification)
            {
                case Notification.InDay:
                    if ((notifications & Notification.InDay) == Notification.InDay)
                        notifications &= ~Notification.InDay;
                    else
                        notifications |= Notification.InDay;
                    break;
                case Notification.ForOneDay:
                    if ((notifications & Notification.ForOneDay) == Notification.ForOneDay)
                        notifications &= ~Notification.ForOneDay;
                    else
                        notifications |= Notification.ForOneDay;
                    break;
                case Notification.ForTwoDays:
                    if ((notifications & Notification.ForTwoDays) == Notification.ForTwoDays)
                        notifications &= ~Notification.ForTwoDays;
                    else
                        notifications |= Notification.ForTwoDays;
                    break;
                case Notification.ForAWeek:
                    if ((notifications & Notification.ForAWeek) == Notification.ForAWeek)
                        notifications &= ~Notification.ForAWeek;
                    else
                        notifications |= Notification.ForAWeek;
                    break;
                default:
                    break;
            }
            tempEvent.Notifications = (int)notifications;
            _eventService.EditEvent(tempEvent);

            List<List<InlineKeyboardButton>> keyboardButtons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.NotificationForOneDay} - {(((Notification)tempEvent.Notifications & Notification.ForOneDay) == Notification.ForOneDay ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangeNotification} {(int)Notification.ForOneDay} {tempEvent.Id}" },
                                                    new InlineKeyboardButton($"{MessageConst.NotificationForTwoDays} - {(((Notification)tempEvent.Notifications & Notification.ForTwoDays) == Notification.ForTwoDays ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangeNotification} {(int)Notification.ForTwoDays} {tempEvent.Id}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.NotificationForAWeek} - {(((Notification)tempEvent.Notifications & Notification.ForAWeek) == Notification.ForAWeek ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangeNotification} {(int)Notification.ForAWeek} {tempEvent.Id}" },
                                                    new InlineKeyboardButton($"{MessageConst.NotificationInDay} - {(((Notification)tempEvent.Notifications & Notification.InDay) == Notification.InDay ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangeNotification} {(int)Notification.InDay} {tempEvent.Id}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton(MessageConst.Next) {CallbackData = $"{CallbackConst.GoToFinalAddingStage} {tempEvent.Id}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton(MessageConst.CancelAdding) { CallbackData = $"{CallbackConst.CancelAdding} {tempEvent.Id}" } }
            };
            await botClient.EditMessageTextAsync(user.ChatId, callbackQuery.Message.MessageId, $"{tempEvent.Date.ToString("dd.MM.yyyy")}\n{tempEvent.Text}\n{MessageConst.EventType}: {tempEvent.Type}\n\n{MessageConst.ChoseNotification}:", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
        }

        private async Task AddEventToCommonCalendarAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            Guid eventId;
            if (!Guid.TryParse(dataSplit[1], out eventId))
                throw new Exception($"Wrong format of event id: {dataSplit[1]}");

            CalendarEvent tempEvent = _eventService.FindEvent(eventId);
            if (tempEvent == null)
                throw new Exception($"Event {eventId} doesn`t exist");

            tempEvent.IsActive = true;
            _eventService.EditEvent(tempEvent);
            List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton>();
            await botClient.EditMessageTextAsync(user.ChatId, callbackQuery.Message.MessageId, $"{MessageConst.YouAddedEventOn} {tempEvent.Date.ToString("dd.MM.yyyy")}\n{tempEvent.Text}\n{MessageConst.EventType}: {tempEvent.Type}\n{tempEvent.Notifications}", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
        }
        private async Task CancelAddingCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            Guid eventId;
            if (!Guid.TryParse(dataSplit[1], out eventId))
                throw new Exception($"Wrong format of event id: {dataSplit[1]}");

            _eventService.DeleteGeneralEvent(eventId);
            List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton>();
            await botClient.EditMessageTextAsync(user.ChatId, callbackQuery.Message.MessageId, MessageConst.EventHasBeenDeleted, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
        }

        private async Task GoToFinalAddingStageCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            Guid eventId;
            if (!Guid.TryParse(dataSplit[1], out eventId))
                throw new Exception($"Wrong format of event id: {dataSplit[1]}");

            CalendarEvent tempEvent = _eventService.FindEvent(eventId);
            if (tempEvent == null)
                throw new Exception($"Event {eventId} doesn`t exist");

            List<List<InlineKeyboardButton>> keyboardButtons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>(){new InlineKeyboardButton(MessageConst.AddEventToCommonCalendar) { CallbackData = $"{CallbackConst.AddEventToCommonCalendar} {tempEvent.Id}" } },
                new List<InlineKeyboardButton>(){new InlineKeyboardButton(MessageConst.CancelAdding) { CallbackData = $"{CallbackConst.CancelAdding} {tempEvent.Id}" } }
            };
            await botClient.EditMessageTextAsync(user.ChatId, callbackQuery.Message.MessageId, $"{tempEvent.Date.ToString("dd.MM.yyyy")}\n{tempEvent.Text}\n{MessageConst.EventType}: {tempEvent.Type}", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));

        }

        private async Task ChangeMyNotificationStatusCallbackQueryAsync(ITelegramBotClient botClient, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.View) != Permission.View)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                _logger.LogDebug("NotEnoughPermissions", user);
                return;
            }
            int status;
            if (!int.TryParse(dataSplit[1], out status))
                throw new Exception($"Wrong format of status: {dataSplit[1]}");

            long chatId;
            if (!long.TryParse(dataSplit[2], out chatId))
                throw new Exception($"Wrong format of chat id: {dataSplit[2]}");

            UserBot managedUser = _userService.FindUser(chatId);
            managedUser.GetNotification = Convert.ToBoolean(status);
            _userService.UpdateUser(managedUser);
            await botClient.SendTextMessageAsync(user.ChatId, $"{(managedUser.GetNotification ? MessageConst.NowYouAreGettingNotifications : MessageConst.NowYouAreNotGettingNotifications)}");
        }
    }
}
