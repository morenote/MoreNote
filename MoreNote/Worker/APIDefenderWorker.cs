using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MoreNote.Common.Config;
using MoreNote.Common.Config.Model;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MoreNoteWorkerService
{
    //����ɨ����վ��־��������������ߵ�IP���������
    public class APIDefenderWorker : BackgroundService
    {
        private readonly ILogger<RandomImagesCrawlerWorker> _logger;

        /// <summary>
        /// ��վ����
        /// </summary>
        private static WebSiteConfig config = ConfigManager.GetWebConfig();

        public APIDefenderWorker()
        {
        }

        private Random random = new Random();

        public APIDefenderWorker(ILogger<RandomImagesCrawlerWorker> logger)
        {
            _logger = logger;
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