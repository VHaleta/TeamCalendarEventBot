using TeamCalendarEventBot.Models.Constants;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeamCalendarEventBot.Domain.Processor.Services
{
    public class MenuService
    {
        public ReplyKeyboardMarkup GetMenuButtons(Permission permission, MenuStage menuStage)
        {
            switch (menuStage)
            {
                case MenuStage.MainMenu:
                    return new ReplyKeyboardMarkup(GetMainMenuButtons(permission));
                case MenuStage.CalendarMenu:
                    return new ReplyKeyboardMarkup(GetCalendarMenuButtons(permission));
                case MenuStage.EditEventMenu:
                    return new ReplyKeyboardMarkup(new List<KeyboardButton>());
                default:
                    break;
            }
            throw new Exception("Menu stage doesnt exists");
        }

        public ReplyKeyboardMarkup NoneAuthKeybord()
        {
            return new ReplyKeyboardMarkup(new KeyboardButton(MessageConst.JoinToBot));
        }

        private List<List<KeyboardButton>> GetCalendarMenuButtons(Permission permission)
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

        private List<List<KeyboardButton>> GetMainMenuButtons(Permission permission)
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
