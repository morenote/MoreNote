using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MoreNote.Logic.Entity.ConfigFile;
using MoreNote.Controllers;
using MoreNote.Logic.Entity;
using MoreNote.Logic.Service;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MoreNoteWorkerService
{
    /// <summary>
    /// ���ͼƬ�ӿ�
    /// </summary>
    public class UpdataImageURLWorker : BackgroundService
    {
        private readonly ILogger<RandomImagesCrawlerWorker> _logger;
        private RandomImageService randomImageService;
       
        /// <summary>
        /// ���ͼƬ�б�
        /// </summary>
       
        /// <summary>
        /// ��վ����
        /// </summary>
        private  readonly WebSiteConfig config;

        private ConfigFileService configFileService;
        public UpdataImageURLWorker()
        {

        }

        private readonly Random random = new Random();

        public UpdataImageURLWorker(ILogger<RandomImagesCrawlerWorker> logger, RandomImageService randomImageService, ConfigFileService configFileService)
        {
            _logger = logger;
            this.randomImageService= randomImageService;
            this.configFileService= configFileService;
            config = configFileService.GetWebConfig();
             size = config.PublicAPI.RandomImageSize;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdatImage().ConfigureAwait(false);
                    int time = DateTime.Now.Hour;
                    //ÿ��60�����ץȡһ��
                    //Ƶ��̫�ߣ�վ����˳�����߹�������
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message, DateTimeOffset.Now);
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken).ConfigureAwait(false);
                }
            }
        }

        private readonly int size;
      
        private async Task UpdatImage()
        {
            var imageTypeList = randomImageService.GetImageTypeList();
            var randomImageList = randomImageService.GetRandomImageList();
            for (int y = 0; y < imageTypeList.Count; y++)
            {
               
                if (!randomImageList.ContainsKey(imageTypeList[y]))
                {
                    randomImageList.Add(imageTypeList[y], new List<RandomImage>(size));
                }
                else
                {
                    //randomImageList[imageTypeList[y]].Clear();
                }
                if (randomImageList[imageTypeList[y]].Count>=size)
                {
                    RandomImage randomImage = randomImageService.GetRandomImage(imageTypeList[y]);
                    randomImageList[imageTypeList[y]][random.Next(0,size)]=randomImage;
                }
                else
                {
                    randomImageList[imageTypeList[y]] = randomImageService.GetRandomImages(imageTypeList[y], size);
                }
               
            }




        }



    }
}
