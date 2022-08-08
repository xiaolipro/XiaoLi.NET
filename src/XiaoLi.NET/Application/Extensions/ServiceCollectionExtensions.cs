using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace XiaoLi.NET.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddStartups(this IServiceCollection services)
        {
            var startups = App.PublicTypes.Where(type =>
            {
                if (!type.IsClass) return false;
                if (type.IsAbstract || type.IsGenericType) return false;

                return typeof(IStartup).IsAssignableFrom(type);
            });

            foreach (var startupType in startups)
            {
                var startup = Activator.CreateInstance(startupType) as IStartup;
                InternalApp.Startups.Add(startup);
                startup.ConfigureServices(services);
            }

            return services;
        }
    }
}