﻿namespace TeamCalendarEventBot.Models.Constants
{
    public static class MessageConst
    {
        public const string JoinToBot = "Запросити авторизацію";
        public const string BackToMainMenu = "Повернутися до головного меню";
        public const string Calendar = "Календар";
        public const string AddEvent = "Додати подію";
        public const string OnWeekEvents = "Події на 7 діб";
        public const string EditEvents = "Редагувати події";
        public const string CheckAuthenticationRequests = "Перевірити запити на авторизацію";
        public const string ManagePermissions = "Керування дозволами";
        public const string Delete = "Видалити";
        public const string GettingNotifications = "Отримування нагадувань";
        public const string WatchTimetable = "Подивитися розклад";
        public const string Info = "Інфо";


        //commands
        public const string StartCommand = "/start";
        public const string RunNotifications = "/runNotifications";

        //permissions
        public const string PermissionView = "Перегляд календаря";
        public const string PermissionOwnCalendar = "Керування своїми подіями";
        public const string PermissionCommonCalendar = "Керування загальними подіями";
        public const string PermissionAuthorizating = "Керування авторизацією";
        public const string PermissionGivingPermissions = "Керування повноваженнями";

        //event type
        public const string ChoseEventType = "Оберіть тип події";
        public const string EventType = "Тип події";
        public const string EventTypeDefault = "Стандартний";
        public const string EventTypeBirthday = "День народження";
        public const string EventTypeDeadline = "Дедлайн";

        //notification
        public const string ChoseNotification = "Оберіть нагадування";
        public const string NotificationInDay = "У цей день";
        public const string NotificationForOneDay = "За 1 добу";
        public const string NotificationForTwoDays = "За 2 доби";
        public const string NotificationForAWeek = "За тиждень";
        public const string NotificationNo = "Без нагадування";

        //reply
        public const string YouAreNotAuthorized = "Ви не авторизовані";
        public const string AuthenticationRequested = "Авторизація запрошена";
        public const string AuthenticationHaveBeenRequested = "Авторизація вже була запрошена, зачекайте підтвердження";
        public const string YouHaveChosenDate = "Ви обрали дату";
        public const string WriteEventText = "Напишіть текст події";
        public const string YouHaveBeenAuthorized = "Вас було авторизовано. Зачекайте поки вам будуть надані повноваження";
        public const string EventsOn = "Події на";
        public const string NoEvents = "Подій нема";
        public const string NotEnoughPermissions = "У вас недостатньо повноважень";
        public const string ChoseAction = "Оберіть дію";
        public const string UnknownMessage = "Невідоме повідомлення\nДля початкового меню напишіть команду /start";
        public const string YouAddedEventOn = "Ви додали подію на";
        public const string NoAuthenticationRequests = "Запитів на авторизацію нема";
        public const string NoUsersExist = "Користувачів не існує";
        public const string ChangePermissions = "Змінити повноваження";
        public const string EventHasBeenDeleted = "Подію видалено";
        public const string AddEventToCommonCalendar = "Додати подію до загального календарю";
        public const string CancelAdding = "Відмінити додавання";
        public const string Yes = "так";
        public const string No = "ні";
        public const string Next = "Далі";
        public const string DoesGetNotifications = "Отримувати нагадування?";
        public const string NowYouAreGettingNotifications = "Тепер ви отримуєте нагадування";
        public const string NowYouAreNotGettingNotifications = "Тепер ви не отримуєте нагадування";
        public const string InfoAboutBot = "Цей бот створений для користування спільним календарем\nВи можете переглядати події, додані адміністраторами бота\nДля отримання більших повноважень/повідомлення про помилки/з пропозиціями змін бота писати @GaletaKatleta";

        public static string UserHaveBeenAuthorized(string username) => $"Користувача @{username} авторизовано";

    }
}
