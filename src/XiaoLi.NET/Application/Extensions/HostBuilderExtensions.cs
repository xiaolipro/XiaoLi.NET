using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace XiaoLi.NET.Application.Extensions
{
    /// <summary>
    /// 主机构建拓展类
    /// </summary>
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureApp(this IHostBuilder builder)
        {
            return builder
                .ConfigureAppConfiguration(((context, configurationBuilder) =>
                {
                    InternalApp.HostingEnvironment = context.HostingEnvironment;
                    // 添加json配置文件
                    AddJsonFiles(configurationBuilder);
                }))
                .ConfigureServices(((context, services) =>
                {
                    InternalApp.Configuration = context.Configuration;
                    InternalApp.Services = services;
                    AddStartups();
                }));
        }

        private static void AddStartups()
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
                startup?.ConfigureServices(InternalApp.Services);
            }
        }

        private static void AddJsonFiles(IConfigurationBuilder configurationBuilder)
        {
            string executeDir = AppContext.BaseDirectory; //程序执行目录

            // 获取执行目录下所有json文件，TopDirectoryOnly不递归
            var jsonFiles = Directory.GetFiles(executeDir, "*.json", SearchOption.TopDirectoryOnly);

            // jsonFiles.Where(file => )
            foreach (var file in jsonFiles)
            {
                if (VaildateJsonFile(file))
                {
                    configurationBuilder.AddJsonFile(file, true, true);
                }
            }
        }

        private static bool VaildateJsonFile(string file)
        {
            var arr = file.Split('.');
            if (arr.Length == 2) return true;
            if (arr.Any(x => string.IsNullOrWhiteSpace(x))) return false;
            return file.EndsWith($".{InternalApp.HostingEnvironment.EnvironmentName}.json");
        }
    }
}