using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MoreNote.Common.Config;
using MoreNote.Common.Config.Model;
using MoreNote.Controllers;
using MoreNote.Logic.Entity;
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
        public static List<string> imageTypeList = new List<string>();
        /// <summary>
        /// ���ͼƬ�б�
        /// </summary>
        public static Dictionary<string, List<RandomImage>> randomImageList = new Dictionary<string, List<RandomImage>>();


        /// <summary>
        /// ��վ����
        /// </summary>
        private static readonly WebSiteConfig config = ConfigManager.GetPostgreSQLConfig();
        public UpdataImageURLWorker()
        {

        }

        private readonly Random random = new Random();

        public UpdataImageURLWorker(ILogger<RandomImagesCrawlerWorker> logger)
        {
            _logger = logger;
            InitList();
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
        private static readonly int size = config.randomImageSize;
        public void InitList()
        {
            lock (this)
            {
                imageTypeList.Add("�����ۺ�1");
                imageTypeList.Add("�����ۺ�2");
                imageTypeList.Add("�����ۺ�3");
                imageTypeList.Add("�����ۺ�4");
                imageTypeList.Add("�����ۺ�5");
                imageTypeList.Add("�����ۺ�6");
                imageTypeList.Add("�����ۺ�7");
                imageTypeList.Add("�����ۺ�8");
                imageTypeList.Add("�����ۺ�9");
                imageTypeList.Add("�����ۺ�10");
                imageTypeList.Add("�����ۺ�11");
                imageTypeList.Add("�����ۺ�12");
                imageTypeList.Add("�����ۺ�13");
                imageTypeList.Add("�����ۺ�14");
                imageTypeList.Add("�����ۺ�15");
                imageTypeList.Add("�����ۺ�16");
                imageTypeList.Add("�����ۺ�17");
                imageTypeList.Add("�����ۺ�18");

                imageTypeList.Add("��Ӱ����1");



                imageTypeList.Add("Ե֮��1");

                imageTypeList.Add("����project1");

                imageTypeList.Add("è��1");

               

                imageTypeList.Add("��Ůǰ��1");

                imageTypeList.Add("�羰ϵ��1");
                imageTypeList.Add("�羰ϵ��2");
                imageTypeList.Add("�羰ϵ��3");
                imageTypeList.Add("�羰ϵ��4");
                imageTypeList.Add("�羰ϵ��5");
                imageTypeList.Add("�羰ϵ��6");
                imageTypeList.Add("�羰ϵ��7");
                imageTypeList.Add("�羰ϵ��8");
                imageTypeList.Add("�羰ϵ��9");
                imageTypeList.Add("�羰ϵ��10");

                imageTypeList.Add("����ϵ��1");
                imageTypeList.Add("����ϵ��2");

                imageTypeList.Add("���շ���1");
                imageTypeList.Add("���շ���2");


                imageTypeList.Add("��װս��1");


                imageTypeList.Add("Pվϵ��1");
                imageTypeList.Add("Pվϵ��2");
                imageTypeList.Add("Pվϵ��3");
                imageTypeList.Add("Pվϵ��4");


                imageTypeList.Add("CGϵ��1");
                imageTypeList.Add("CGϵ��2");
                imageTypeList.Add("CGϵ��3");
                imageTypeList.Add("CGϵ��4");
                imageTypeList.Add("CGϵ��5");


                imageTypeList.Add("�����ȷ�");

                imageTypeList.Add("������ҫ");

                imageTypeList.Add("��Ůд��1");
                imageTypeList.Add("��Ůд��2");
                imageTypeList.Add("��Ůд��3");
                imageTypeList.Add("��Ůд��4");
                imageTypeList.Add("��Ůд��5");
                imageTypeList.Add("��Ůд��6");


                imageTypeList.Add("����ˮ����");
                imageTypeList.Add("����");
                imageTypeList.Add("��Ʒ��ŮͼƬ");
                imageTypeList.Add("�ձ�COS�й�COS");
                imageTypeList.Add("��Ůӳ��");

            }

        }
        private async Task UpdatImage()
        {

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
                    RandomImage randomImage = RandomImageService.GetRandomImage(imageTypeList[y]);
                    randomImageList[imageTypeList[y]][random.Next(0,size)]=randomImage;
                }
                else
                {
                    randomImageList[imageTypeList[y]] = RandomImageService.GetRandomImages(imageTypeList[y], size);
                }
               
            }




        }



    }
}
