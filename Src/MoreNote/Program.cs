﻿using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoreNote.Common.Utils;
using MoreNote.Logic.Service;
using System.Linq;

namespace MoreNote
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitSecret();//初始化安全密钥
            CreateHostBuilder(args).Build().Run();
        }

        private static void InitSecret()
        {
            //每次启动程序都会重新初始化Secret
            ConfigFileService configFileService = new ConfigFileService();
            configFileService.WebConfig.SecurityConfig.Secret = RandomTool.CreatSafeRandomBase64(32);
            configFileService.Save();
            System.Console.WriteLine("安全密钥已经初始化成功");
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureLogging((context, loggingBuilder) =>
            {
                 //loggingBuilder.ClearProviders();
                loggingBuilder.AddFilter("System", LogLevel.Warning);
                loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);//过滤掉系统自带的System，Microsoft开头的，级别在Warning以下的日志
                loggingBuilder.AddLog4Net("config/log4net.config"); //会读取appsettings.json的Logging:LogLevel:Default级别
            });
    }
}