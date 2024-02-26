﻿using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using MoreNote.Common.Utils;
using MoreNote.Logic.Service;

using NLog.Web;

using System;
using System.Collections.Generic;

namespace MoreNote
{
	public class Program
	{
		public static void Main(string[] args)
		{

			//设置读取指定位置的nlog.config文件
			if (RuntimeEnvironment.IsWindows)
			{
				NLogBuilder.ConfigureNLog("nlog-windows.config");
			}
			else
			{
				NLogBuilder.ConfigureNLog("nlog-linux.config");
			}

			var host = CreateHostBuilder(args).Build();
			var map = GetArgsMap(args);

			DeployService deployService = new DeployService(host);
			if (map.Keys.Contains("m"))
			{
				var cmd = map["m"];

				switch (cmd)
				{
					case "GenSecret":
						//deployService.InitSecret();
						return;
					case "MigrateDatabase":
						deployService.MigrateDatabase();

						return;
					default:
						Console.WriteLine("unkown cmd");
						return;
				}
			}

			host.Run();
		}

		private static Dictionary<string, string> GetArgsMap(string[] args)
		{
			var map = new Dictionary<string, string>();
			for (int index = 0; index < args.Length; index++)
			{
				string arg = args[index];
				if (arg.StartsWith("-"))
				{
					if ((index + 1) < args.Length && (!args[index + 1].StartsWith("-")))
					{
						map.Add(arg.Replace("-", ""), args[index + 1]);
						index++;
					}
					else
					{
						map.Add(arg.Replace("-", ""), null);
					}
				}
			}
			return map;
		}

		/// <summary>
		/// 初始化安全秘钥
		/// </summary>
		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)

			.UseServiceProviderFactory(new AutofacServiceProviderFactory())
			.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.UseUrls("http://*:5000").UseStartup<Startup>();
			})

			.UseNLog();



	}
}