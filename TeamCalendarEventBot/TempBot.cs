using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeamCalendarEventBot
{
    public class TempBot
    {
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received {update.Type}");
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceivedAsync(botClient, update),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static async Task GoTo2MessageAsync(ITelegramBotClient botClient, Update update)
        {
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Реквізити:\n\n5375414121769466 Моно\n5168752020277839 Приват\nУ коментарі до платіжки потрібно додати свій @нікнейм з телеграму", replyMarkup: new ReplyKeyboardRemove());
        }

        private static async Task GoTo1MessageAsync(ITelegramBotClient botClient, Update update)
        {
            List<KeyboardButton> keyboardButtons = new List<KeyboardButton>();
            keyboardButtons.Add(new KeyboardButton("До сплати"));
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Доступ до кінця семестру коштує 300₴");
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Помісячна сплата коштує 100₴ за вересень або жовтень та 150₴ за листопад.", replyMarkup: new ReplyKeyboardMarkup(keyboardButtons));
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine("unknownUpdate");
            return Task.CompletedTask;
        }

        private static async Task BotOnMessageReceivedAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Text: {update.Message.Text}");
            var action = update.Message.Text switch
            {
                "/start" => StartAsync(botClient, update),
                "Перейти до тарифів" => GoTo1MessageAsync(botClient, update),
                "До сплати" => GoTo2MessageAsync(botClient, update),
                _ => UnknownMessageAsync(botClient, update),
            };
            try
            {
                await action;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
        }

        private static async Task UnknownMessageAsync(ITelegramBotClient botClient, Update update)
        {
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Невідоме повідомлення\nДля початку напишіть /start");
        }

        private static async Task StartAsync(ITelegramBotClient botClient, Update update)
        {
            List<KeyboardButton> keyboardButtons = new List<KeyboardButton> {
                new KeyboardButton("Перейти до тарифів")
            };
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Вітаю! Тут ви зможете отримати доступ до ексклюзивного ресурсу з усіма конспектами, відповідями на тестові завдання та прикладами до домашніх завдань.", replyMarkup: new ReplyKeyboardMarkup(keyboardButtons));
        }

    }
}
