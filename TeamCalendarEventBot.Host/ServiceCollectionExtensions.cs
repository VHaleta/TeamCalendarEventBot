﻿using Microsoft.Extensions.DependencyInjection;
using TeamCalendarEventBot.DAL.Automapper;
using TeamCalendarEventBot.DAL.Persistence;
using TeamCalendarEventBot.DAL.Persistence.FileProvider;
using TeamCalendarEventBot.DAL.Repositories;
using TeamCalendarEventBot.Domain.Listener;
using TeamCalendarEventBot.Domain.Processor;
using TeamCalendarEventBot.Domain.Processor.Handlers;
using TeamCalendarEventBot.Domain.Processor.Services;
using TeamCalendarEventBot.Domain.Repositories;
using TeamCalendarEventBot.SqsConnection;

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
            services.AddTransient<ICalendarService, CalendarService>();
            services.AddTransient<IMenuService, MenuService>();

            services.AddSingleton<IEventService, EventService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<INotificationService, NotificationService>();

            // Add Handlers
            services.AddSingleton<IMessageHandler, MessageHandler>();
            services.AddSingleton<ICallbackQueryHandler, CallbackQueryHandler>();
            services.AddSingleton<IUnknownUpdateHandler, UnknownUpdateHandler>();

            // Add Processor
            services.AddTransient<BotProcessor>();

            // Add listener
            services.AddTransient<IListener, Listener>();
            services.AddSingleton<ISqsCommunication, SqsCommunication>();

            return services;
        }

        public static IServiceCollection AddAutomapperConfiguration(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserMapperProfile));
            services.AddAutoMapper(typeof(EventMapperProfile));

            return services;
        }
    }
}
