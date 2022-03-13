using System;
using System.Collections.Generic;
using System.Text;
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
                buttons.Add(new List<KeyboardButton> { new KeyboardButton("События на сегодня"), new KeyboardButton("События на завтра")});
                buttons.Add(new List<KeyboardButton> { new KeyboardButton("События на неделю") });
            }

            if (((permission & Permission.OwnCalendar) == Permission.OwnCalendar) || ((permission & Permission.CommonCalendar) == Permission.CommonCalendar))
                buttons.Add(new List<KeyboardButton> { new KeyboardButton("Редактировать события") });

            buttons.Add(new List<KeyboardButton> { new KeyboardButton(MessageConst.BackToMainMenu) });

            return buttons;
        }

        private static List<List<KeyboardButton>> GetMainMenuButtons(Permission permission)
        {
            List<List<KeyboardButton>> buttons = new List<List<KeyboardButton>>();

            if ((permission & Permission.View) == Permission.View)
                buttons.Add(new List<KeyboardButton> { new KeyboardButton(MessageConst.Calendar) });

            if ((permission & Permission.OwnCalendar) == Permission.OwnCalendar)
                buttons.Add(new List<KeyboardButton> { new KeyboardButton("Добавить событие(для себя)") });

            if ((permission & Permission.CommonCalendar) == Permission.CommonCalendar)
                buttons.Add(new List<KeyboardButton> { new KeyboardButton(MessageConst.AddEventForAll) });

            if (((permission & Permission.OwnCalendar) == Permission.OwnCalendar) || ((permission & Permission.CommonCalendar) == Permission.CommonCalendar))
                buttons.Add(new List<KeyboardButton> { new KeyboardButton("Редактировать события") });

            return buttons;
        }
    }
}
