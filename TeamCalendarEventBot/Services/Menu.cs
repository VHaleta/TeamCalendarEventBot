using System.Collections.Generic;
using TeamCalendarEventBot.Constants;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeamCalendarEventBot.Services
{
    public static class Menu
    {
        static ReplyKeyboardMarkup keyboard;
        public static ReplyKeyboardMarkup GetMenuButtons(Permission permission, MenuStage menuStage)
        {
            switch (menuStage)
            {
                case MenuStage.MainMenu:
                    keyboard = new ReplyKeyboardMarkup(GetMainMenuButtons(permission));
                    break;
                case MenuStage.CalendarMenu:
                    keyboard = new ReplyKeyboardMarkup(GetCalendarMenuButtons(permission));
                    break;
                case MenuStage.EditEventMenu:
                    break;
                default:
                    break;
            }
            return keyboard;
        }

        public static ReplyKeyboardMarkup NoneAuthKeybord()
        {
            return new ReplyKeyboardMarkup(new KeyboardButton(MessageConst.JoinToBot));
        }

        private static List<List<KeyboardButton>> GetCalendarMenuButtons(Permission permission)
        {
            List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>();

            if ((permission & Permission.View) == Permission.View)
            {
                buttons.Add(new List<KeyboardButton> { new KeyboardButton(MessageConst.OnWeekEvents) });
                buttons.Add(new List<KeyboardButton> { new KeyboardButton(MessageConst.Calendar) });
            }

            if (((permission & Permission.OwnCalendar) == Permission.OwnCalendar) || ((permission & Permission.CommonCalendar) == Permission.CommonCalendar))
                buttons.Add(new List<KeyboardButton> { new KeyboardButton(MessageConst.AddEvent), new KeyboardButton(MessageConst.EditEvents) });

            buttons.Add(new List<KeyboardButton> { new KeyboardButton(MessageConst.BackToMainMenu) });

            return buttons;
        }

        private static List<List<KeyboardButton>> GetMainMenuButtons(Permission permission)
        {
            List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>();

            if ((permission & Permission.View) == Permission.View)
                buttons.Add(new List<KeyboardButton> { new KeyboardButton(MessageConst.Calendar) });

            if ((permission & Permission.View) == Permission.View)
                buttons.Add(new List<KeyboardButton> { new KeyboardButton(MessageConst.GettingNotifications) });

            if ((permission & Permission.View) == Permission.View)
                buttons.Add(new List<KeyboardButton> { new KeyboardButton(MessageConst.WatchTimetable) });

            if ((permission & Permission.View) == Permission.View)
                buttons.Add(new List<KeyboardButton> { new KeyboardButton(MessageConst.Info) });

            if ((permission & Permission.Authorizating) == Permission.Authorizating)
                buttons.Add(new List<KeyboardButton> { new KeyboardButton(MessageConst.CheckAuthenticationRequests) });

            if ((permission & Permission.GivingPermissions) == Permission.GivingPermissions)
                buttons.Add(new List<KeyboardButton> { new KeyboardButton(MessageConst.ManagePermissions) });

            return buttons;
        }
    }
}
