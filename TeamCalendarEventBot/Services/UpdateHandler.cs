using System;
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
                        await OnAddingEventMessageAsync(botClient, user, message.Text);
                        break;
                    default:
                        break;
                };
                return;
            }
            var action = message.Text! switch
            {
                //Commands
                MessageConst.Start => StartupMessageAsync(botClient, user),
                MessageConst.Authentication => AuthenticationCommandAsync(botClient, user),
                MessageConst.Commands => CommandsCommandAsync(botClient, user),
                MessageConst.ManagePermissions => ManagePermissionsAsync(botClient, user),
                //Startapmenu
                MessageConst.Calendar => CalendarMessageAsync(botClient, user),
                //CalendarMenu
                MessageConst.AddEventForAll => AddEventForAllMessageAsync(botClient, user),
                MessageConst.ResendCalendar => CalendarMessageAsync(botClient, user),
                MessageConst.OnWeekEvents => OnWeekEventsMessageAsync(botClient, user),
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
            switch (dataSplit[0])
            {
                case CallbackConst.ChangeMonth:
                    await ChangeMonthCallbackQueryAsync(botClient, callbackQuery, user, dataSplit);
                    break;
                case CallbackConst.GetEvents:
                    await ShowCalendarEventsByDateCallbackQueryAsync(botClient, user, dataSplit);
                    break;
                case CallbackConst.Adding:
                    await AddingEventCallbackQueryAsync(botClient, user, dataSplit);
                    break;
                case CallbackConst.Authentication:
                    await AuthenticateUserCallbackQueryAsync(botClient, user, dataSplit);
                    break;
                case CallbackConst.ManagePermissions:
                    await ManagePermissionsCallbackQueryAsync(botClient, callbackQuery, user, dataSplit);
                    break;
                case CallbackConst.ChangePermission:
                    await ChangePermissionCallbackQueryAsync(botClient, callbackQuery, user, dataSplit);
                    break;
                default:
                    Console.WriteLine($"Unknown callback type: {dataSplit[0]}");
                    break;

            }
        }
        #endregion

        #region Message
        private static async Task CalendarMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
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
        //TODO: adding events
        private static async Task AddEventForAllMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.Calendar, replyMarkup: Calendar.GetAddingEventKetboard(DateTime.Today));
        }

        private static async Task OnWeekEventsMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            await Services.EventHandler.ShowCalendarEventsByWeekAsync(botClient, DateTime.Today, user);
        }

        private static async Task OnAddingEventMessageAsync(ITelegramBotClient botClient, UserBot user, string message)
        {
            user.TempCalendarEvent.Text = message;
            await Services.EventHandler.AddGeneralEventAsync(botClient, user, user.TempCalendarEvent);
            await botClient.SendTextMessageAsync(user.ChatId, $"{MessageConst.YouAddedEventOn} {user.TempCalendarEvent.Date.ToString("dd.MM.yyyy")}");
            user.UserStatus = UserStatus.None;
            UserHandler.UpdateUser(user);
        }
        private static async Task AuthenticationCommandAsync(ITelegramBotClient botClient, UserBot user)
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
        private static async Task CommandsCommandAsync(ITelegramBotClient botClient, UserBot user)
        {
            await botClient.SendTextMessageAsync(user.ChatId, $"{MessageConst.Start}\n{MessageConst.Commands}\n{MessageConst.Authentication}\n{MessageConst.ManagePermissions}");
        }

        private static async Task ManagePermissionsAsync(ITelegramBotClient botClient, UserBot user)
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

        private static async Task AddingEventCallbackQueryAsync(ITelegramBotClient botClient, UserBot user, string[] dataSplit)
        {
            DateTime date;
            if (!DateTime.TryParse(dataSplit[1], out date))
            {
                Console.WriteLine($"Wrong format of date: {dataSplit[1]}");
                return;
            }
            user.UserStatus = UserStatus.Adding;
            user.TempCalendarEvent = new CalendarEvent() { Date = date };
            await botClient.SendTextMessageAsync(user.ChatId, $"{MessageConst.YouHaveChosenDate}: {date.ToString("dd.MM.yyyy")}\n{MessageConst.WriteEventText}:");
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
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionView} - {((((Permission)managedUser.Permissions & Permission.View) == Permission.View) ? "так" : "ні")}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.View} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionOwnCalendar} - {((((Permission)managedUser.Permissions & Permission.OwnCalendar) == Permission.OwnCalendar) ? "так" : "ні")}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.OwnCalendar} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionCommonCalendar} - {((((Permission)managedUser.Permissions & Permission.CommonCalendar) == Permission.CommonCalendar) ? "так" : "ні")}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.CommonCalendar} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionAuthorizating} - {((((Permission)managedUser.Permissions & Permission.Authorizating) == Permission.Authorizating) ? "так" : "ні")}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.Authorizating} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionGivingPermissions} - {((((Permission)managedUser.Permissions & Permission.GivingPermissions) == Permission.GivingPermissions) ? "так" : "ні")}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.GivingPermissions} {managedUser.ChatId}" } }
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
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionView} - {((((Permission)managedUser.Permissions & Permission.View) == Permission.View) ? "так" : "ні")}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.View} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionOwnCalendar} - {((((Permission)managedUser.Permissions & Permission.OwnCalendar) == Permission.OwnCalendar) ? "так" : "ні")}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.OwnCalendar} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionCommonCalendar} - {((((Permission)managedUser.Permissions & Permission.CommonCalendar) == Permission.CommonCalendar) ? "так" : "ні")}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.CommonCalendar} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionAuthorizating} - {((((Permission)managedUser.Permissions & Permission.Authorizating) == Permission.Authorizating) ? "так" : "ні")}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.Authorizating} {managedUser.ChatId}" } },
                new List<InlineKeyboardButton>(){ new InlineKeyboardButton($"{MessageConst.PermissionGivingPermissions} - {((((Permission)managedUser.Permissions & Permission.GivingPermissions) == Permission.GivingPermissions) ? "так" : "ні")}") { CallbackData = $"{CallbackConst.ChangePermission} {(int)Permission.GivingPermissions} {managedUser.ChatId}" } }
            };
            await botClient.EditMessageReplyMarkupAsync(user.ChatId, callbackQuery.Message.MessageId, replyMarkup: new InlineKeyboardMarkup(keyboardButtons));

        }
        #endregion
    }
}
