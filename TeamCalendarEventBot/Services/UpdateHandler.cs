﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamCalendarEventBot.Constants;
using TeamCalendarEventBot.Models;
using TeamCalendarEventBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeamCalendarEventBot.Sevices
{
    public class UpdateHandler
    {
        //TODO:Review permissions
        #region UpdateTypes
        public static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }
        public static async Task BotOnMessageReceivedAsync(ITelegramBotClient botClient, Message message, UserBot user)
        {
            if (message.Type != MessageType.Text) return;
            Console.WriteLine($"Receive update type: Message: {message.Text}\nchat id: {user.ChatId} username: {user.Username}\n----------------------------------------------------");

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
                MessageConst.CheckAuthenticationRequests => AuthenticationMessageAsync(botClient, user),
                MessageConst.ManagePermissions => ManagePermissionsMessageAsync(botClient, user),
                MessageConst.GettingNotifications => GettingNotificationsMessageAsync(botClient, user),
                //CalendarMenu
                MessageConst.AddEvent => AddEventMessageAsync(botClient, user),
                MessageConst.OnWeekEvents => OnWeekEventsMessageAsync(botClient, user),
                MessageConst.EditEvents => EditEventsMessageAsync(botClient, user),
                //General
                MessageConst.BackToMainMenu => StartupMessageAsync(botClient, user),
                _ => UnknownMessageAsync(botClient, user)
            };

            await action;
        }

        public static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user)
        {
            Console.WriteLine($"Receive update type: CallbackQuery: {callbackQuery.Data}\nchat id: {user.ChatId} username: {user.Username}\n----------------------------------------------------");
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
            await action;
        }
        #endregion

        #region Message
        private static async Task CalendarMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            if (((Permission)user.Permissions & Permission.View) != Permission.View)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.Calendar, replyMarkup: Calendar.GetCalendarKeyboard(DateTime.Today));
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.ChoseAction, replyMarkup: Menu.GetMenuButtons((Permission)user.Permissions, MenuStage.CalendarMenu));
        }

        private static async Task StartupMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.ChoseAction, replyMarkup: Menu.GetMenuButtons((Permission)user.Permissions, MenuStage.MainMenu));
        }

        private static async Task UnknownMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            await botClient.SendTextMessageAsync(user.ChatId, text: MessageConst.UnknownMessage);
        }

        private static async Task AddEventMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            if ((((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar) || ((Permission)user.Permissions & Permission.OwnCalendar) != Permission.OwnCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.Calendar, replyMarkup: Calendar.GetAddingEventKetboard(DateTime.Today));
        }

        private static async Task OnWeekEventsMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            if (((Permission)user.Permissions & Permission.View) != Permission.View)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            await Services.EventHandler.ShowCalendarEventsByWeekAsync(botClient, DateTime.Today, user);
        }

        private static async Task OnAddingTextToEventMessageAsync(ITelegramBotClient botClient, UserBot user, string message)
        {
            CalendarEvent tempEvent = new CalendarEvent() { Date = user.TempDate, Text = message };
            await Services.EventHandler.AddGeneralEventAsync(botClient, user, tempEvent);
            List<List<InlineKeyboardButton>> keyboardButtons = new List<List<InlineKeyboardButton>> {
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton(MessageConst.EventTypeDefault) { CallbackData = $"{CallbackConst.AddEventType} {(int)CalendarEventType.Default} {tempEvent.Id}"}, new InlineKeyboardButton(MessageConst.EventTypeDeadline) { CallbackData = $"{CallbackConst.AddEventType} {(int)CalendarEventType.Deadline} {tempEvent.Id}"} },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton(MessageConst.EventTypeBirthday) { CallbackData = $"{CallbackConst.AddEventType} {(int)CalendarEventType.Birthday} {tempEvent.Id}"} },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton(MessageConst.CancelAdding) { CallbackData = $"{CallbackConst.CancelAdding} {tempEvent.Id}" } }
            };
            await botClient.SendTextMessageAsync(user.ChatId, $"{tempEvent.Date.ToString("dd.MM.yyyy")}\n{tempEvent.Text}\n\n{MessageConst.ChoseEventType}:", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
            user.UserStatus = UserStatus.None;
            UserHandler.UpdateUser(user);
        }
        private static async Task AuthenticationMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            if (((Permission)user.Permissions & Permission.Authorizating) != Permission.Authorizating)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            List<UserBot> users = UserHandler.GetAllRequestedUsers();
            if (users.Count == 0)
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NoAuthenticationRequests);
            foreach (var item in users)
            {
                List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton> {
                    new InlineKeyboardButton("Авторизувати") { CallbackData = $"{CallbackConst.Authentication} {item.ChatId}"} };
                await botClient.SendTextMessageAsync(user.ChatId, $"@{item.Username}", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
            }
        }
        private static async Task ManagePermissionsMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            if (((Permission)user.Permissions & Permission.GivingPermissions) != Permission.GivingPermissions)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            List<UserBot> users = UserHandler.GetAllUsersExceptMe(user);
            if (users.Count == 0)
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NoUsersExist);
            foreach (UserBot item in users)
            {
                List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton> {
                    new InlineKeyboardButton(MessageConst.ChangePermissions) { CallbackData = $"{CallbackConst.ManagePermissions} {item.ChatId}"} };
                await botClient.SendTextMessageAsync(user.ChatId, $"@{item.Username}", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
            }
        }

        private static async Task EditEventsMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            if ((((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar) || ((Permission)user.Permissions & Permission.OwnCalendar) != Permission.OwnCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.Calendar, replyMarkup: Calendar.GetEditEventKeyboard(DateTime.Today));
        }

        private static Task RunNotificationsCommand(ITelegramBotClient botClient, UserBot user)
        {
            NotificationHandler.StartNotifications(botClient);
            return Task.CompletedTask;
        }

        private static async Task GettingNotificationsMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            if (((Permission)user.Permissions & Permission.View) != Permission.View)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton> {
                    new InlineKeyboardButton(MessageConst.Yes) { CallbackData = $"{CallbackConst.ChangeMyNotificationStatus} {1} {user.ChatId}"},
                    new InlineKeyboardButton(MessageConst.No) { CallbackData = $"{CallbackConst.ChangeMyNotificationStatus} {0} {user.ChatId}"}};
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.DoesGetNotifications, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
        }
        #endregion

        #region CallbackQuery
        private static async Task ChangeMonthCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            int month = 0, year = 0;
            if (!int.TryParse(dataSplit[1], out month) || !int.TryParse(dataSplit[2], out year))
            {
                Console.WriteLine($"Wrong format of month or year: {dataSplit[1]} {dataSplit[2]}");
                return;
            }
            DateTime date = new DateTime(year, month, 1);
            await botClient.EditMessageReplyMarkupAsync(chatId: user.ChatId, callbackQuery.Message.MessageId, replyMarkup: Calendar.GetCalendarKeyboard(date));
        }
        private static async Task ShowCalendarEventsByDateCallbackQueryAsync(ITelegramBotClient botClient, UserBot user, string[] dataSplit)
        {
            DateTime date;
            if (!DateTime.TryParse(dataSplit[1], out date))
            {
                Console.WriteLine($"Wrong format of date: {dataSplit[1]}");
                return;
            }
            await Services.EventHandler.ShowCalendarEventsByDateAsync(botClient, date, user);
        }

        private static async Task AddingEventCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            DateTime date;
            if (!DateTime.TryParse(dataSplit[1], out date))
            {
                Console.WriteLine($"Wrong format of date: {dataSplit[1]}");
                return;
            }
            user.UserStatus = UserStatus.Adding;
            user.TempDate = date;
            List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton>();
            await botClient.EditMessageTextAsync(user.ChatId, callbackQuery.Message.MessageId, $"{MessageConst.YouHaveChosenDate}: {date.ToString("dd.MM.yyyy")}\n{MessageConst.WriteEventText}:", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
            UserHandler.UpdateUser(user);
        }

        private static async Task AuthenticateUserCallbackQueryAsync(ITelegramBotClient botClient, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.Authorizating) != Permission.Authorizating)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            long chatId;
            if (!long.TryParse(dataSplit[1], out chatId))
            {
                Console.WriteLine($"Wrong format of chatId: {dataSplit[1]}");
                return;
            }
            UserBot authUser = UserHandler.FindUser(chatId);
            if (authUser == null)
            {
                Console.WriteLine($"User {chatId} doesn`t exist");
                return;
            }
            authUser.Auth = AuthenticationState.Approved;
            UserHandler.UpdateUser(authUser);
            await botClient.SendTextMessageAsync(authUser.ChatId, MessageConst.YouHaveBeenAuthorized);
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.UserHaveBeenAuthorized(authUser.Username));
        }
        private static async Task ManagePermissionsCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.GivingPermissions) != Permission.GivingPermissions)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            long chatId;
            if (!long.TryParse(dataSplit[1], out chatId))
            {
                Console.WriteLine($"Wrong format of chatId: {dataSplit[1]}");
                return;
            }
            UserBot managedUser = UserHandler.FindUser(chatId);
            if (managedUser == null)
            {
                Console.WriteLine($"User {chatId} doesn`t exist");
                return;
            }
            List<List<InlineKeyboardButton>> keyboardButtons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionView} - {((((Permission)managedUser.Permissions & Permission.View) == Permission.View) ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.View} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionOwnCalendar} - {((((Permission)managedUser.Permissions & Permission.OwnCalendar) == Permission.OwnCalendar) ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.OwnCalendar} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionCommonCalendar} - {((((Permission)managedUser.Permissions & Permission.CommonCalendar) == Permission.CommonCalendar) ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.CommonCalendar} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionAuthorizating} - {((((Permission)managedUser.Permissions & Permission.Authorizating) == Permission.Authorizating) ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.Authorizating} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionGivingPermissions} - {((((Permission)managedUser.Permissions & Permission.GivingPermissions) == Permission.GivingPermissions) ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.GivingPermissions} {managedUser.ChatId}" } }
            };
            await botClient.EditMessageReplyMarkupAsync(chatId: user.ChatId, callbackQuery.Message.MessageId, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));

        }
        private static async Task ChangePermissionCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.GivingPermissions) != Permission.GivingPermissions)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            long chatId;
            if (!long.TryParse(dataSplit[2], out chatId))
            {
                Console.WriteLine($"Wrong format of chatId: {dataSplit[2]}");
                return;
            }
            int permission;
            if (!int.TryParse(dataSplit[1], out permission))
            {
                Console.WriteLine($"Wrong format of permission: {dataSplit[1]}");
                return;
            }
            UserBot managedUser = UserHandler.FindUser(chatId);
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
            UserHandler.UpdateUser(managedUser);
            List<List<InlineKeyboardButton>> keyboardButtons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionView} - {((((Permission)managedUser.Permissions & Permission.View) == Permission.View) ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.View} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionOwnCalendar} - {((((Permission)managedUser.Permissions & Permission.OwnCalendar) == Permission.OwnCalendar) ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.OwnCalendar} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionCommonCalendar} - {((((Permission)managedUser.Permissions & Permission.CommonCalendar) == Permission.CommonCalendar) ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.CommonCalendar} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionAuthorizating} - {((((Permission)managedUser.Permissions & Permission.Authorizating) == Permission.Authorizating) ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.Authorizating} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionGivingPermissions} - {((((Permission)managedUser.Permissions & Permission.GivingPermissions) == Permission.GivingPermissions) ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.GivingPermissions} {managedUser.ChatId}" } }
            };
            await botClient.EditMessageReplyMarkupAsync(user.ChatId, callbackQuery.Message.MessageId, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));

        }

        private static async Task DeleteEventCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            Guid Id;
            if (!Guid.TryParse(dataSplit[1], out Id))
            {
                Console.WriteLine($"Wrong format of event id: {dataSplit[1]}");
                return;
            }
            Services.EventHandler.DeleteGeneralEvent(Id);
            List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton>();
            await botClient.EditMessageTextAsync(user.ChatId, callbackQuery.Message.MessageId, MessageConst.EventHasBeenDeleted, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));

        }

        private static async Task EditEventsCallbackQueryAsync(ITelegramBotClient botClient, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            DateTime date;
            if (!DateTime.TryParse(dataSplit[1], out date))
            {
                Console.WriteLine($"Wrong format of date: {dataSplit[1]}");
                return;
            }
            await Services.EventHandler.EditCalendarEventsByDateAsync(botClient, date, user);
        }

        private static Task UnknownCallbackQuery(ITelegramBotClient botClient, UserBot user)
        {
            Console.WriteLine("Unknown callback query");
            return Task.CompletedTask;
        }

        private static async Task AddEventTypeCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            int type;
            if (!int.TryParse(dataSplit[1], out type))
            {
                Console.WriteLine($"Wrong format of event type: {dataSplit[1]}");
                return;
            }
            Guid eventId;
            if (!Guid.TryParse(dataSplit[2], out eventId))
            {
                Console.WriteLine($"Wrong format of event id: {dataSplit[1]}");
                return;
            }
            CalendarEvent tempEvent = Services.EventHandler.FindEvent(eventId);
            if (tempEvent == null) return;
            tempEvent.Type = (CalendarEventType)type;
            Services.EventHandler.EditEvent(tempEvent);
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
        private static async Task ChangeNotificationCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            int notification;
            if (!int.TryParse(dataSplit[1], out notification))
            {
                Console.WriteLine($"Wrong format of notification: {dataSplit[1]}");
                return;
            }
            Guid eventId;
            if (!Guid.TryParse(dataSplit[2], out eventId))
            {
                Console.WriteLine($"Wrong format of event id: {dataSplit[1]}");
                return;
            }
            CalendarEvent tempEvent = Services.EventHandler.FindEvent(eventId);
            if (tempEvent == null) return;

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
            Services.EventHandler.EditEvent(tempEvent);

            List<List<InlineKeyboardButton>> keyboardButtons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.NotificationForOneDay} - {((((Notification)tempEvent.Notifications & Notification.ForOneDay) == Notification.ForOneDay) ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangeNotification} {(int)Notification.ForOneDay} {tempEvent.Id}" },
                                                    new InlineKeyboardButton($"{MessageConst.NotificationForTwoDays} - {((((Notification)tempEvent.Notifications & Notification.ForTwoDays) == Notification.ForTwoDays) ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangeNotification} {(int)Notification.ForTwoDays} {tempEvent.Id}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.NotificationForAWeek} - {((((Notification)tempEvent.Notifications & Notification.ForAWeek) == Notification.ForAWeek) ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangeNotification} {(int)Notification.ForAWeek} {tempEvent.Id}" },
                                                    new InlineKeyboardButton($"{MessageConst.NotificationInDay} - {((((Notification)tempEvent.Notifications & Notification.InDay) == Notification.InDay) ? MessageConst.Yes : MessageConst.No)}") { CallbackData = $"{CallbackConst.ChangeNotification} {(int)Notification.InDay} {tempEvent.Id}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton(MessageConst.Next) {CallbackData = $"{CallbackConst.GoToFinalAddingStage} {tempEvent.Id}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton(MessageConst.CancelAdding) { CallbackData = $"{CallbackConst.CancelAdding} {tempEvent.Id}" } }
            };
            await botClient.EditMessageTextAsync(user.ChatId, callbackQuery.Message.MessageId, $"{tempEvent.Date.ToString("dd.MM.yyyy")}\n{tempEvent.Text}\n{MessageConst.EventType}: {tempEvent.Type}\n\n{MessageConst.ChoseNotification}:", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
        }

        private static async Task AddEventToCommonCalendarAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            Guid eventId;
            if (!Guid.TryParse(dataSplit[1], out eventId))
            {
                Console.WriteLine($"Wrong format of event id: {dataSplit[1]}");
                return;
            }
            CalendarEvent tempEvent = Services.EventHandler.FindEvent(eventId);
            if (tempEvent == null) return;
            tempEvent.IsActive = true;
            Services.EventHandler.EditEvent(tempEvent);
            List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton>();
            await botClient.EditMessageTextAsync(user.ChatId, callbackQuery.Message.MessageId, $"{MessageConst.YouAddedEventOn} {tempEvent.Date.ToString("dd.MM.yyyy")}\n{tempEvent.Text}\n{MessageConst.EventType}: {tempEvent.Type}\n{tempEvent.Notifications}", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
        }
        private static async Task CancelAddingCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            Guid eventId;
            if (!Guid.TryParse(dataSplit[1], out eventId))
            {
                Console.WriteLine($"Wrong format of event id: {dataSplit[1]}");
                return;
            }
            Services.EventHandler.DeleteGeneralEvent(eventId);
            List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton>();
            await botClient.EditMessageTextAsync(user.ChatId, callbackQuery.Message.MessageId, MessageConst.EventHasBeenDeleted, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));
        }

        private static async Task GoToFinalAddingStageCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.CommonCalendar) != Permission.CommonCalendar)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            Guid eventId;
            if (!Guid.TryParse(dataSplit[1], out eventId))
            {
                Console.WriteLine($"Wrong format of event id: {dataSplit[1]}");
                return;
            }
            CalendarEvent tempEvent = Services.EventHandler.FindEvent(eventId);
            if (tempEvent == null) return;
            List<List<InlineKeyboardButton>> keyboardButtons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>(){new InlineKeyboardButton(MessageConst.AddEventToCommonCalendar) { CallbackData = $"{CallbackConst.AddEventToCommonCalendar} {tempEvent.Id}" } },
                new List<InlineKeyboardButton>(){new InlineKeyboardButton(MessageConst.CancelAdding) { CallbackData = $"{CallbackConst.CancelAdding} {tempEvent.Id}" } }
            };
            await botClient.EditMessageTextAsync(user.ChatId, callbackQuery.Message.MessageId, $"{tempEvent.Date.ToString("dd.MM.yyyy")}\n{tempEvent.Text}\n{MessageConst.EventType}: {tempEvent.Type}", replyMarkup: new InlineKeyboardMarkup(keyboardButtons));

        }

        private static async Task ChangeMyNotificationStatusCallbackQueryAsync(ITelegramBotClient botClient, UserBot user, string[] dataSplit)
        {
            if (((Permission)user.Permissions & Permission.View) != Permission.View)
            {
                await botClient.SendTextMessageAsync(user.ChatId, MessageConst.NotEnoughPermissions);
                return;
            }
            int status;
            if (!int.TryParse(dataSplit[1], out status))
            {
                Console.WriteLine($"Wrong format of status: {dataSplit[1]}");
                return;
            }
            long chatId;
            if (!long.TryParse(dataSplit[2], out chatId))
            {
                Console.WriteLine($"Wrong format of chat id: {dataSplit[1]}");
                return;
            }
            UserBot managedUser = UserHandler.FindUser(chatId);
            managedUser.GetNotification = Convert.ToBoolean(status);
            UserHandler.UpdateUser(managedUser);
            await botClient.SendTextMessageAsync(user.ChatId, $"{(managedUser.GetNotification ? MessageConst.NowYouAreGettingNotifications : MessageConst.NowYouAreNotGettingNotifications)}");
        }
        #endregion
    }
}
