using AutoMapper;
using TeamCalendarEventBot.DAL.DataModels;
using TeamCalendarEventBot.Models.Constants;
using TeamCalendarEventBot.Models.Models;

namespace TeamCalendarEventBot.DAL.Automapper
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<UserBotData, UserBot>()
                .ForMember(dest => dest.ChatId,
                    opt => opt.MapFrom(bot => bot.ChatId))
                .ForMember(dest => dest.Username,
                    opt => opt.MapFrom(bot => bot.Username))
                .ForMember(dest => dest.FirstName,
                    opt => opt.MapFrom(bot => bot.FirstName))
                .ForMember(dest => dest.LastName,
                    opt => opt.MapFrom(bot => bot.LastName))
                .ForMember(dest => dest.Permissions,
                    opt => opt.MapFrom(bot => bot.Permissions))
                .ForMember(dest => dest.Active,
                    opt => opt.MapFrom(bot => bot.Active))
                .ForMember(dest => dest.GetNotification,
                    opt => opt.MapFrom(bot => bot.GetNotification))
                .ForMember(dest => dest.Auth,
                    opt => opt.MapFrom(bot => (AuthenticationState)bot.Auth))
                .ForMember(dest => dest.UserStatus,
                    opt => opt.MapFrom(bot => (UserStatus)bot.UserStatus))
                .ForMember(dest => dest.MenuStage,
                    opt => opt.MapFrom(bot => (MenuStage)bot.MenuStage))
                .ForMember(dest => dest.TempDate,
                    opt => opt.MapFrom(bot => bot.TempDate));

            CreateMap<UserBot, UserBotData>()
                .ForMember(dest => dest.ChatId,
                    opt => opt.MapFrom(bot => bot.ChatId))
                .ForMember(dest => dest.Username,
                    opt => opt.MapFrom(bot => bot.Username))
                .ForMember(dest => dest.FirstName,
                    opt => opt.MapFrom(bot => bot.FirstName))
                .ForMember(dest => dest.LastName,
                    opt => opt.MapFrom(bot => bot.LastName))
                .ForMember(dest => dest.Permissions,
                    opt => opt.MapFrom(bot => bot.Permissions))
                .ForMember(dest => dest.Active,
                    opt => opt.MapFrom(bot => bot.Active))
                .ForMember(dest => dest.GetNotification,
                    opt => opt.MapFrom(bot => bot.GetNotification))
                .ForMember(dest => dest.Auth,
                    opt => opt.MapFrom(bot => (int)bot.Auth))
                .ForMember(dest => dest.UserStatus,
                    opt => opt.MapFrom(bot => (int)bot.UserStatus))
                .ForMember(dest => dest.MenuStage,
                    opt => opt.MapFrom(bot => (int)bot.MenuStage))
                .ForMember(dest => dest.TempDate,
                    opt => opt.MapFrom(bot => bot.TempDate));
        }
    }
}
