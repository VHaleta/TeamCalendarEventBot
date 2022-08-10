namespace TeamCalendarEventBot.Constants
{
    public static class MessageConst
    {
        public const string JoinToBot = "Запросити авторизацію";
        public const string BackToMainMenu = "Повернутися до головного меню";
        public const string Calendar = "Календар";
        public const string AddEventForAll = "Додати подію(для всіх)";
        public const string OnWeekEvents = "Події на тиждень";
        public const string EditEvent = "Редагувати події";
        public const string AddEventForMe = "Додати подію(для себе)";
        public const string CheckAuthenticationRequests = "Переверити запити на авторизацію";
        public const string ManagePermissions = "Керування дозволами";

        //commands
        public const string StartCommand = "/start";
        public const string AuthenticationCommand = "/authentication";
        public const string CommandsCommand = "/commands";
        public const string ManagePermissionsCommand = "/managePermissions";

        //permissions
        public const string PermissionView = "Перегляд календаря";
        public const string PermissionOwnCalendar = "Керування своїми подіями";
        public const string PermissionCommonCalendar = "Керування загальними подіями";
        public const string PermissionAuthorizating = "Керування авторизацією";
        public const string PermissionGivingPermissions = "Керування повноваженнями";

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
        public const string NoAuthenticationRequests = "Запитів на авторизацію нема";
        public const string NoUsersExist = "Користувачів не існує";
        public const string ChangePermissions = "Змінити повноваження";

        public static string UserHaveBeenAuthorized(string username) => $"Користувача @{username} авторизовано";

    }
}
