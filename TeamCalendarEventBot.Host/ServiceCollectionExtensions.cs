using Microsoft.Extensions.DependencyInjection;
using TeamCalendarEventBot.DAL.Persistence;
using TeamCalendarEventBot.DAL.Persistence.FileProvider;
using TeamCalendarEventBot.DAL.Repositories;
using TeamCalendarEventBot.Domain.Processor;
using TeamCalendarEventBot.Domain.Processor.Handlers;
using TeamCalendarEventBot.Domain.Processor.Services;
using TeamCalendarEventBot.Domain.Repositories;

namespace TeamCalendarEventBot.Host
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDalDependencies(this IServiceCollection services)
        {
            // Add persistence
            services.AddTransient<IFileProvider, FileProvider>();
            services.AddSingleton<IEventDao, EventDao>();
            services.AddSingleton<IUserBotDao, UserBotDao>();

            // Add repositories
            services.AddTransient<IEventRepository, EventRepository>();
            services.AddTransient<IUserBotRepository, UserBotRepository>();

            return services;
        }

        public static IServiceCollection AddDomainDependencies(this IServiceCollection services)
        {
            // Add Services
            services.AddTransient<CalendarService>();
            services.AddTransient<EventService>();
            services.AddTransient<MenuService>();
            services.AddTransient<UserService>();

            services.AddSingleton<NotificationService>();

            // Add Handlers
            services.AddSingleton<MessageHandler>();
            services.AddSingleton<CallbackQueryHandler>();
            services.AddSingleton<UnknownUpdateHandler>();

            // Add Processor
            services.AddSingleton<BotProcessor>();

            return services;
        }
    }
}
