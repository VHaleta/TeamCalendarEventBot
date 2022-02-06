using System;
using System.Threading.Tasks;
using TeamCalendarEventBot.Models;
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

            var action = message.Text! switch
            {
                //"/inline" => SendInlineKeyboard(botClient, message),
                //"/keyboard" => SendReplyKeyboard(botClient, message),
                //"/remove" => RemoveKeyboard(botClient, message),
                //"/photo" => SendFile(botClient, message),
                //"/request" => RequestContactAndLocation(botClient, message),
                _ => UnknownMessageAsync(botClient, message)
            };
            Message sentMessage = await action;
            Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");

        }
        private static async Task<Message> UnknownMessageAsync(ITelegramBotClient botClient, Message message)
        {
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Unknown Message");
        }


        //private TelegramBotClient client;
        //IDataClient dataClient;
        //List<UserBot> allUsers;
        //public UpdateHandler(TelegramBotClient client)
        //{
        //    this.client = client;
        //    dataClient = new JsonFileDataClient();
        //    allUsers = dataClient.UserInfoDataProvider.GetAllUsers();
        //}
        //public void ProcessUpdates()
        //{
        //    int offset = 0;
        //    while (true)
        //    {
        //        var updates = client.GetUpdatesAsync(offset).Result;
        //        if (updates != null && updates.Count() > 0)
        //        {
        //            allUsers = dataClient.UserInfoDataProvider.GetAllUsers();
        //            foreach (var update in updates)
        //            {
        //                ProcessUpdate(update);
        //                offset = update.Id + 1;
        //            }
        //        }
        //        Thread.Sleep(500);
        //    }
        //}

        //private void ProcessUpdate(Update update)
        //{
        //    UserBot userBot = UserCheck(update);
        //    client.SendTextMessageAsync(userBot.ChatId, "Меню", Menu.GetMenuButtons((Permission)userBot.Permissions, MenuStage.MainMenu));
        //}
    }
}
