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
            Console.WriteLine($"Receive update type: Message: {message.Text}\nchat id: {user.ChatId} username: {user.Username}");

            var action = message.Text! switch
            {
                //"/inline" => SendInlineKeyboard(botClient, message),
                //"/keyboard" => SendReplyKeyboard(botClient, message),
                //"/remove" => RemoveKeyboard(botClient, message),
                //"/photo" => SendFile(botClient, message),
                //"/request" => RequestContactAndLocation(botClient, message),
                "/start" => StartupMessageAsync(botClient, message, user),
                MessageConst.BackToMainMenu => StartupMessageAsync(botClient, message, user),
                MessageConst.Calendar => CalendarMessageAsync(botClient, message, user),
                _ => UnknownMessageAsync(botClient, message, user)
            };

            Message sentMessage = await action;
        }
        public static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user)
        {
            Console.WriteLine($"Receive update type: CallbackQuery: {callbackQuery.Data}\nchat id: {user.ChatId} username: {user.Username}");
            string data = callbackQuery.Data;
            DateTime date;
            if(!DateTime.TryParse(data, out date))
            {
                string[] dataSplit = data.Split();
                switch (dataSplit[0])
                {
                    case CallbackConst.ChangeMonth:
                        int month;
                        if(!int.TryParse(dataSplit[1], out month))
                        {
                            Console.WriteLine($"Wrong format of month: {dataSplit[1]}");
                        }
                        await ChangeMonthAsync(botClient, callbackQuery , user, month);
                        break;
                }
            }
            else
            {
                Console.WriteLine("It was date: " + date.ToString());
            }
        }
        #endregion

        #region Message
        private static async Task<Message> CalendarMessageAsync(ITelegramBotClient botClient, Message message, UserBot user)
        {
            await botClient.SendTextMessageAsync(user.ChatId, MessageConst.Calendar, replyMarkup: Calendar.GetCalendarKeyboard(DateTime.Today));
            return await botClient.SendTextMessageAsync(chatId: user.ChatId, "Выберите действие", replyMarkup: Menu.GetMenuButtons((Permission)user.Permissions, MenuStage.CalendarMenu));
        }

        private static async Task<Message> StartupMessageAsync(ITelegramBotClient botClient, Message message, UserBot user)
        {
            return await botClient.SendTextMessageAsync(chatId: user.ChatId, "Выберите действие", replyMarkup: Menu.GetMenuButtons((Permission)user.Permissions, MenuStage.MainMenu));
        }

        private static async Task<Message> UnknownMessageAsync(ITelegramBotClient botClient, Message message, UserBot user)
        {
            return await botClient.SendTextMessageAsync(chatId: user.ChatId, text: "Неизвестное сообщение\nДля начального меню введите команду /start");
        }
        #endregion

        #region CallbackQuery
        private static async Task ChangeMonthAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserBot user, int month)
        {
            if (month == 13) month = 1;
            if (month == 0) month = 12;
            //TODO: Add year changing 
            DateTime date = new DateTime(DateTime.Today.Year, month, 1);
            await botClient.EditMessageReplyMarkupAsync(chatId: user.ChatId, callbackQuery.Message.MessageId, replyMarkup: Calendar.GetCalendarKeyboard(date));
        }

        #endregion
    }
}
