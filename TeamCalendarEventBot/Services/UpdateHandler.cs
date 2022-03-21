using System;
using System.Threading.Tasks;
using TeamCalendarEventBot.Constants;
using TeamCalendarEventBot.Models;
using TeamCalendarEventBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeamCalendarEventBot.Sevices
{
    public class UpdateHandler
    {
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

            var action = message.Text! switch
            {
                //Commands
                "/start" => StartupMessageAsync(botClient, user),
                //Startapmenu
                MessageConst.Calendar => CalendarMessageAsync(botClient, user),
                //CalendarMenu
                MessageConst.AddEventForAll => AddEventForAllAsync(botClient, user),
                MessageConst.ResendCalendar => CalendarMessageAsync(botClient, user),
                MessageConst.OnWeekEvents => OnWeekEventsAsync(botClient, user),
                //General
                MessageConst.BackToMainMenu => StartupMessageAsync(botClient, user),
                _ => UnknownMessageAsync(botClient, user)
            };

            await action;
        }

        public static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user)
        {
            Console.WriteLine($"Receive update type: CallbackQuery: {callbackQuery.Data}\nchat id: {user.ChatId} username: {user.Username}\n----------------------------------------------------");
            string data = callbackQuery.Data;
            DateTime date;
            if (!DateTime.TryParse(data, out date))
            {
                string[] dataSplit = data.Split();
                switch (dataSplit[0])
                {
                    case CallbackConst.ChangeMonth:
                        int month = 0, year = 0;
                        if (!int.TryParse(dataSplit[1], out month) || !int.TryParse(dataSplit[2], out year))
                        {
                            Console.WriteLine($"Wrong format of month or year: {dataSplit[1]} {dataSplit[2]}");
                        }
                        await ChangeMonthAsync(botClient, callbackQuery, user, month, year);
                        break;
                }
            }
            else
            {
                await ShowCalendarEventsByDateAsync(botClient, date, user);
            }
        }
        #endregion

        #region Message
        private static async Task CalendarMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.Calendar, replyMarkup: Calendar.GetCalendarKeyboard(DateTime.Today));
            await botClient.SendTextMessageAsync(chatId: user.ChatId, "Выберите действие", replyMarkup: Menu.GetMenuButtons((Permission)user.Permissions, MenuStage.CalendarMenu));
        }

        private static async Task StartupMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            await botClient.SendTextMessageAsync(chatId: user.ChatId, "Выберите действие", replyMarkup: Menu.GetMenuButtons((Permission)user.Permissions, MenuStage.MainMenu));
        }

        private static async Task UnknownMessageAsync(ITelegramBotClient botClient, UserBot user)
        {
            await botClient.SendTextMessageAsync(chatId: user.ChatId, text: "Неизвестное сообщение\nДля начального меню введите команду /start");
        }
        //TODO: adding events
        private static async Task AddEventForAllAsync(ITelegramBotClient botClient, UserBot user)
        {
//            await Services.EventHandler.AddGeneralEvent();
        }

        private static async Task OnWeekEventsAsync(ITelegramBotClient botClient, UserBot user)
        {
            await Services.EventHandler.ShowCalendarEventsByWeekAsync(botClient, DateTime.Today, user);
        }

        #endregion

        #region CallbackQuery
        private static async Task ChangeMonthAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, int month, int year)
        {
            DateTime date = new DateTime(year, month, 1);
            await botClient.EditMessageReplyMarkupAsync(chatId: user.ChatId, callbackQuery.Message.MessageId, replyMarkup: Calendar.GetCalendarKeyboard(date));
        }
        private static async Task ShowCalendarEventsByDateAsync(ITelegramBotClient botClient, DateTime date, UserBot user)
        {
            await Services.EventHandler.ShowCalendarEventsByDateAsync(botClient, date, user);
        }
        #endregion
    }
}
