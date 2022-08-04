﻿namespace TeamCalendarEventBot.Constants
{
    public static class MessageConst
    {
        public const string JoinToBot = "Запросити авторизацію";
        public const string BackToMainMenu = "Повернутися до головного меню";
        public const string Calendar = "Календар";
        public const string AddEventForAll = "Додати подію(для всіх)";
        public const string ResendCalendar = "Перенадіслати календар";
        public const string OnWeekEvents = "Події на тиждень";
        public const string EditEvent = "Редагувати події";
        public const string AddEventForMe = "Додати подію(для себе)";

        //commands
        public const string Start = "/start";
        public const string Authentication = "/authentication";
        public const string Commands = "/commands";

        //reply
        public const string YouAreNotAuthorized = "Ви не авторизовані";
        public const string AuthenticationRequested = "Авторизація запрошена";
        public const string YouHaveChosenDate = "Ви обрали дату";
        public const string WriteEventText = "Напишіть текст події";
        public const string YouHaveBeenAuthorized = "Вас було авторизовано";
        public const string EventsOn = "Події на";
        public const string NoEvents = "Подій нема";
        public const string NotEnoughPermissions = "У вас недостатньо повноважень";
        public const string ChoseAction = "Оберіть дію";
        public const string UnknownMessage = "Невідоме повідомлення\nДля початкового меню напишіть команду /start";
        public const string YouAddedEventOn = "Ви додали подію на";
        public static string UserHaveBeenAuthorized(string username) => $"Користувача @{username} авторизовано";

    }
}
