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
using MoreNote.Common.Config;
using MoreNote.Common.Config.Model;
using MoreNote.Common.Util;
using MoreNote.Common.Utils;
using MoreNote.Controllers;
using MoreNote.Logic.Entity;
using MoreNote.Logic.Service;
using UpYunLibrary;

namespace MoreNoteWorkerService
{
    public class InitRandomImagesWorker : BackgroundService
    {
        private readonly ILogger<InitRandomImagesWorker> _logger;
        /// <summary>
        /// ��վ����
        /// </summary>
        static WebSiteConfig config = ConfigManager.GetPostgreSQLConfig();
        /// <summary>
        /// ������
        /// </summary>
        static UpYun upyun = new UpYun(config.upyunBucket, config.upyunUsername, config.upyunPassword);
        public InitRandomImagesWorker()
        {

        }
        List<string > typeList=new List<string>();
        Random random = new Random();

        public InitRandomImagesWorker(ILogger<InitRandomImagesWorker> logger)
        {
            _logger = logger;
            typeList.Add("�����ۺ�1");
            typeList.Add("�����ۺ�2");
            typeList.Add("�����ۺ�3");
            typeList.Add("�����ۺ�4");
            typeList.Add("�����ۺ�5");
            typeList.Add("�����ۺ�6");
            typeList.Add("�����ۺ�7");
            typeList.Add("�����ۺ�8");
            typeList.Add("�����ۺ�9");
            typeList.Add("�����ۺ�10");
            typeList.Add("�����ۺ�11");
            typeList.Add("�����ۺ�12");
            typeList.Add("�����ۺ�13");
            typeList.Add("�����ۺ�14");
            typeList.Add("�����ۺ�15");
            typeList.Add("�����ۺ�16");
            typeList.Add("�����ۺ�17");
            typeList.Add("�����ۺ�18");

            typeList.Add("��Ӱ����1");

      
            
            typeList.Add("Ե֮��1");

            typeList.Add("����project1");

            typeList.Add("����ϵ��1");
            typeList.Add("����ϵ��2");

            typeList.Add("��Ůǰ��1");

            typeList.Add("�羰ϵ��1");
            typeList.Add("�羰ϵ��2");
            typeList.Add("�羰ϵ��3");
            typeList.Add("�羰ϵ��4");
            typeList.Add("�羰ϵ��5");
            typeList.Add("�羰ϵ��6");
            typeList.Add("�羰ϵ��7");
            typeList.Add("�羰ϵ��8");
            typeList.Add("�羰ϵ��9");

            typeList.Add("���շ���1");
            typeList.Add("���շ���2");


            typeList.Add("��װս��1");


            typeList.Add("Pվϵ��1");
            typeList.Add("Pվϵ��2");
            typeList.Add("Pվϵ��3");


            typeList.Add("CGϵ��1");
            typeList.Add("CGϵ��2");
            typeList.Add("CGϵ��3");
            typeList.Add("CGϵ��4");
            typeList.Add("CGϵ��5");


            typeList.Add("�����ȷ�");

            typeList.Add("������ҫ");

            typeList.Add("��Ůд��1");
            typeList.Add("��Ůд��2");
            typeList.Add("��Ůд��3");
            typeList.Add("��Ůд��4");
            typeList.Add("��Ůд��5");
            typeList.Add("��Ůд��6");


            typeList.Add("����ˮ����");
            typeList.Add("����");
            typeList.Add("��Ʒ��ŮͼƬ");
            typeList.Add("�ձ�COS�й�COS");
            typeList.Add("��Ůӳ��");
            

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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
                    var number = random.Next(typeList.Count);
                    await GetHttpWebRequestForAnYaAsync(typeList[number]).ConfigureAwait(false);
                    int time = DateTime.Now.Hour;
                    //ÿ��60�����ץȡһ��
                    //Ƶ��̫�ߣ�վ����˳�����߹�������
                    await Task.Delay(TimeSpan.FromSeconds(config.Reptile_Delay_Second), stoppingToken).ConfigureAwait(false);
                }
                catch(Exception ex)
                {
                    _logger.LogInformation(ex.Message, DateTimeOffset.Now);
                    await Task.Delay(TimeSpan.FromSeconds(config.Reptile_Delay_Second), stoppingToken).ConfigureAwait(false);
                }
               
            
            }
        }
        private async Task GetHttpWebRequestForAnYaAsync(string type)
        {
          
            string url = "";
            if (type.Equals("��Ůӳ��"))
            {
                url = "https://api.r10086.com:8443/��Ůӳ��.php?password=20";
            }
            else
            {
                url = $"https://api.r10086.com:8443/" + type + ".php";
            }
            //��������
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //���Referer��Ϣ
            request.Headers.Add(HttpRequestHeader.Referer, "http://www.bz08.cn/");
            //αװ�ɹȸ������ 
            //request.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36");
            request.Headers.Add(HttpRequestHeader.UserAgent, "I am a cute web crawler");
            //���cookie��֤��Ϣ
            Cookie cookie = new Cookie("PHPSESSID", "s9gajue8h7plf7n5ab8fehiuoq");
            cookie.Domain = "api.r10086.com";
            if (request.CookieContainer == null)
            {
                request.CookieContainer = new CookieContainer();
            }
            request.CookieContainer.Add(cookie);
            //���������ȡHttp��Ӧ
            HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync();
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
            while ((i = await receiveStream.ReadAsync(buffer1, 0, buffer1.Length)) > 0)
            {
                stmMemory.Write(buffer1, 0, i);
            }
           
            //д�����
            string name = System.IO.Path.GetFileName(originalString);
            byte[] imageBytes = stmMemory.ToArray();
            string fileSHA1 = SHAEncrypt_Helper.Hash1Encrypt(imageBytes);
            //�ϴ���������
            if (!RandomImageService.Exist(type, fileSHA1))
            {
                upyun.writeFile($"/upload/{SHAEncrypt_Helper.MD5Encrypt(type)}/{fileSHA1}{Path.GetExtension(name)}", imageBytes, true);
                RandomImage randomImage = new RandomImage()
                {
                    RandomImageId = SnowFlake_Net.GenerateSnowFlakeID(),
                    TypeName = type,
                    TypeNameMD5 = SHAEncrypt_Helper.MD5Encrypt(type),
                    TypeNameSHA1 = SHAEncrypt_Helper.Hash1Encrypt(type),
                    FileName = name,
                    FileNameMD5 = SHAEncrypt_Helper.MD5Encrypt(name),
                    FileNameSHA1 = SHAEncrypt_Helper.Hash1Encrypt(name),
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
