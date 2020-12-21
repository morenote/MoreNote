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
    /// <summary>
    /// ���ͼƬ�ӿڵ�����
    /// </summary>
    public class RandomImagesCrawlerWorker : BackgroundService
    {
        private readonly ILogger<RandomImagesCrawlerWorker> _logger;
        /// <summary>
        /// ��վ����
        /// </summary>
        static WebSiteConfig config = ConfigFileService.GetWebConfig();
        /// <summary>
        /// ������
        /// </summary>
        static UpYun upyun = new UpYun(config.UpYunCDN.UpyunBucket, config.UpYunCDN.UpyunUsername, config.UpYunCDN.UpyunPassword);
        public RandomImagesCrawlerWorker()
        {

        }
       
        Random random = new Random();
       
        public RandomImagesCrawlerWorker(ILogger<RandomImagesCrawlerWorker> logger)
        {
            _logger = logger;
          
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var imageTypeList = RandomImageService.GetImageTypeList();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    ////���
                    //var randomImageList = APIController.RandomImageList;
                    //var size = randomImageList.Count;
                    //int max = 120;
                    //string name ="";
                    //GetHttpWebRequest("�����ۺ�2", out name);
                    var number = random.Next(imageTypeList.Count);
                    await GetHttpWebRequestForAnYaAsync(imageTypeList[number]).ConfigureAwait(false);
                    int time = DateTime.Now.Hour;
                    //ÿ��60�����ץȡһ��
                    //Ƶ��̫�ߣ�վ����˳�����߹�������
                    await Task.Delay(TimeSpan.FromSeconds(config.Spiders.Reptile_Delay_Second), stoppingToken).ConfigureAwait(false);
                }
                catch(Exception ex)
                {
                    _logger.LogInformation(ex.Message, DateTimeOffset.Now);
                    await Task.Delay(TimeSpan.FromSeconds(config.Spiders.Reptile_Delay_Second), stoppingToken).ConfigureAwait(false);
                }
               
            
            }
        }
        private async Task GetHttpWebRequestForAnYaAsync(string type)
        {
          
            string url = "";
            if (type.Equals("��Ůӳ��"))
            {
                url = "https://api.r10086.com/��Ůӳ��.php?password=20";
            }
            else
            {
                url = $"https://api.r10086.com/" + type + ".php";
            }
            //��������
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //���Referer��Ϣ
            request.Headers.Add(HttpRequestHeader.Referer, "http://www.bz08.cn/");
            //αװ�ɹȸ������ 
            //request.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36");
            request.Headers.Add(HttpRequestHeader.UserAgent, "Power By www.morenote.top");
            //���cookie��֤��Ϣ
            Cookie cookie = new Cookie("PHPSESSID", "s9gajue8h7plf7n5ab8fehiuoq");
            cookie.Domain = "api.r10086.com";
            if (request.CookieContainer == null)
            {
                request.CookieContainer = new CookieContainer();
            }
            request.CookieContainer.Add(cookie);
            //���������ȡHttp��Ӧ
            HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync().ConfigureAwait(false);
            //HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync().ConfigureAwait(false));

            var originalString = response.ResponseUri.OriginalString;
            Console.WriteLine(originalString);
            //��ȡ��Ӧ��
            Stream receiveStream = response.GetResponseStream();
            
            //��ȡ��Ӧ���ĳ���
            int length = (int)response.ContentLength;
            //��ȡ���ڴ�
            MemoryStream stmMemory = new MemoryStream();
            byte[] buffer1 = new byte[length];
            int i;
            //���ֽ�������뵽Byte ��
            while ((i = await receiveStream.ReadAsync(buffer1, 0, buffer1.Length).ConfigureAwait(false)) > 0)
            {
                stmMemory.Write(buffer1, 0, i);
            }
            //д�����
            string name = System.IO.Path.GetFileName(originalString);
            byte[] imageBytes = stmMemory.ToArray();
            string fileSHA1 = SHAEncryptHelper.Hash1Encrypt(imageBytes);
            //�ϴ���������
            if (!RandomImageService.Exist(type, fileSHA1))
            {
                upyun.writeFile($"/upload/{SHAEncryptHelper.MD5Encrypt(type)}/{fileSHA1}{Path.GetExtension(name)}", imageBytes, true);
                RandomImage randomImage = new RandomImage()
                {
                    RandomImageId = SnowFlakeNet.GenerateSnowFlakeID(),
                    TypeName = type,
                    TypeNameMD5 = SHAEncryptHelper.MD5Encrypt(type),
                    TypeNameSHA1 = SHAEncryptHelper.Hash1Encrypt(type),
                    FileName = name,
                    FileNameMD5 = SHAEncryptHelper.MD5Encrypt(name),
                    FileNameSHA1 = SHAEncryptHelper.Hash1Encrypt(name),
                    FileSHA1 = fileSHA1,
                    Sex = false,
                };
                //��¼�����ݿ�
                await RandomImageService.InsertImageAsync(randomImage).ConfigureAwait(false);

            }
            
            //name = $"{dir}{dsc}upload{dsc}{type}{dsc}{name}";
            //if (!Directory.Exists($"{dir}{dsc}upload{dsc}{type}"))
            //{
            //    Directory.CreateDirectory($"{dir}{dsc}upload{dsc}{type}");
            //}
            //if (!System.IO.File.Exists(name))
            //{
            //    FileStream file = new FileStream(name, FileMode.Create, FileAccess.ReadWrite);
            //    file.Write(stmMemory.ToArray());
            //    file.Flush();
            //    file.Close();
            //}
            //FileStream file = new FileStream("1.jpg",FileMode.Create, FileAccess.ReadWrite);
            //�ر���
            stmMemory.Close();
            receiveStream.Close();
            response.Close();
           
        }
       
    
    }
}
