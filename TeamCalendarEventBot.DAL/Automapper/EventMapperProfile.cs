using AutoMapper;
using TeamCalendarEventBot.DAL.DataModels;
using TeamCalendarEventBot.Models.Constants;
using TeamCalendarEventBot.Models.Models;

namespace TeamCalendarEventBot.DAL.Automapper
{
    public class EventMapperProfile : Profile
    {
        public EventMapperProfile()
        {
            CreateMap<CalendarEventData, CalendarEvent>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(bot => bot.Id))
                .ForMember(dest => dest.Date,
                    opt => opt.MapFrom(bot => bot.Date))
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(bot => (CalendarEventType)bot.Type))
                .ForMember(dest => dest.Notifications,
                    opt => opt.MapFrom(bot => bot.Notifications))
                .ForMember(dest => dest.Text,
                    opt => opt.MapFrom(bot => bot.Text))
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(bot => bot.IsActive));

            CreateMap<CalendarEvent, CalendarEventData>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(bot => bot.Id))
                .ForMember(dest => dest.Date,
                    opt => opt.MapFrom(bot => bot.Date))
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(bot => (int)bot.Type))
                .ForMember(dest => dest.Notifications,
                    opt => opt.MapFrom(bot => bot.Notifications))
                .ForMember(dest => dest.Text,
                    opt => opt.MapFrom(bot => bot.Text))
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(bot => bot.IsActive));

        }
    }
}
