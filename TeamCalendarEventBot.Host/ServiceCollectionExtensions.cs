using Microsoft.Extensions.DependencyInjection;
using TeamCalendarEventBot.DAL.Persistence;
using TeamCalendarEventBot.DAL.Persistence.FileProvider;
using TeamCalendarEventBot.DAL.Repositories;
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
    }
}
