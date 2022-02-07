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
        public static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }
        public static async Task BotOnMessageReceivedAsync(ITelegramBotClient botClient, Message message, UserBot user)
        {
            Console.WriteLine($"Receive message type: {message.Type}");
            if (message.Type != MessageType.Text)
                return;
            if (!await UserHandler.IsUserAuthorizedAsync(botClient, message, user)) return;

                var action = message.Text! switch
            {
                //"/inline" => SendInlineKeyboard(botClient, message),
                //"/keyboard" => SendReplyKeyboard(botClient, message),
                //"/remove" => RemoveKeyboard(botClient, message),
                //"/photo" => SendFile(botClient, message),
                //"/request" => RequestContactAndLocation(botClient, message),
                "/start" => StartupMessageAsync(botClient, message, user),
                "Вернуться в главное меню" => StartupMessageAsync(botClient, message, user),
                "Календарь" => CalendarMessageAsync(botClient, message, user),
                _ => UnknownMessageAsync(botClient, message, user)
            };
            Message sentMessage = await action;
            Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");

        }

        private static async Task<Message> CalendarMessageAsync(ITelegramBotClient botClient, Message message, UserBot user)
        {
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
    }
}
