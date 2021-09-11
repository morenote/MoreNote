using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoreNote.Logic.Entity;
using MoreNote.Logic.Entity.ConfigFile;
using MoreNote.Logic.Service;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly WebSiteConfig config;

        /// <summary>
        /// ÿ��ϵ�е����ͼƬ����
        /// </summary>
        private readonly int _randomImageSize;

        private ConfigFileService configFileService;

        public UpdataImageURLWorker()
        {
        }

        private readonly Random random = new Random();

        public UpdataImageURLWorker(ILogger<RandomImagesCrawlerWorker> logger, RandomImageService randomImageService, ConfigFileService configFileService)
        {
            _logger = logger;
            this.randomImageService = randomImageService;
            this.configFileService = configFileService;
            config = configFileService.WebConfig;
            _randomImageSize = config.PublicAPI.RandomImageSize;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int delaySecondTime = configFileService.WebConfig.PublicAPI.UpdateTime;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdatImage().ConfigureAwait(false);
                    await Task.Delay(TimeSpan.FromSeconds(delaySecondTime), stoppingToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message, DateTimeOffset.Now);
                    await Task.Delay(TimeSpan.FromSeconds(delaySecondTime), stoppingToken).ConfigureAwait(false);
                }
            }
        }

        private async Task UpdatImage()
        {
            var imageTypeList = randomImageService.GetImageTypeList();
            var randomImageList = randomImageService.GetRandomImageList();

            for (int y = 0; y < imageTypeList.Count; y++)
            {
                if (!randomImageList.ContainsKey(imageTypeList[y]))
                {
                    randomImageList.Add(imageTypeList[y], new List<RandomImage>(_randomImageSize));
                }

                if (randomImageList[imageTypeList[y]].Count >= _randomImageSize)
                {
                    RandomImage randomImage = randomImageService.GetRandomImage(imageTypeList[y]);
                    randomImageList[imageTypeList[y]][random.Next(0, randomImageList.Count)] = randomImage;
                }
                else
                {
                    RandomImage randomImage = randomImageService.GetRandomImage(imageTypeList[y]);
                    randomImageList[imageTypeList[y]].Add(randomImage);
                }
            }
        }
    }
}