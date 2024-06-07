namespace TeamCalendarEventBot.Models.Constants
{
    [Flags]
    public enum Permission
    {
        Unknown = 0,
        View = 1,
        OwnCalendar = 2,
        CommonCalendar = 4,
        Authorizating = 8,
        GivingPermissions = 16,
    }
}
