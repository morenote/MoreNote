using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MoreNote.Logic.Entity.ConfigFile;
using MoreNote.Common.Utils;
using MoreNote.Common.Utils;
using MoreNote.Controllers;
using MoreNote.Logic.Entity;
using MoreNote.Logic.Service;

using UpYunLibrary;

namespace MoreNoteWorkerService
{
    //����ɨ����վ��־��������������ߵ�IP���������
    public class APIDefenderWorker : BackgroundService
    {
        private readonly ILogger<RandomImagesCrawlerWorker> _logger;
        private ConfigFileService configFileService;
        /// <summary>
        /// ��վ����
        /// </summary>
        static WebSiteConfig config ;
        public APIDefenderWorker()
        {

        }

        Random random = new Random();

        public APIDefenderWorker(ILogger<RandomImagesCrawlerWorker> logger,DependencyInjectionService dependencyInjectionService)
        {
            _logger = logger;
            configFileService=dependencyInjectionService.ServiceProvider.GetService(typeof(ConfigFileService))as ConfigFileService;
            
                config = configFileService.GetWebConfig();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Check().ConfigureAwait(false);
                    int time = DateTime.Now.Hour;
                    //ÿ��60�����ץȡһ��
                    //Ƶ��̫�ߣ�վ����˳�����߹�������
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message, DateTimeOffset.Now);
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken).ConfigureAwait(false);
                }
            }
        }
        private async Task Check()
        {

        }



    }
}
