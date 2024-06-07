using Telegram.Bot;

namespace TeamCalendarEventBot.Domain.Processor.Services
{
    public interface INotificationService
    {
        public void StartNotifications(ITelegramBotClient botClient);
    }
}
