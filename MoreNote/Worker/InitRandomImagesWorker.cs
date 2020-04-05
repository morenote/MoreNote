using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoreNote.Controllers;

namespace MoreNoteWorkerService
{
    public class InitRandomImagesWorker : BackgroundService
    {
        private readonly ILogger<InitRandomImagesWorker> _logger;

        public InitRandomImagesWorker()
        {
        }

        public InitRandomImagesWorker(ILogger<InitRandomImagesWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //���
                var randomImageList = APIController.RandomImageList;
                var size = randomImageList.Count;
                int max = 120;
                string name ="";
                GetHttpWebRequest("�����ۺ�2", out name);
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
        private static byte[] GetHttpWebRequest(string type,out string key)
        {
            //��������
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://api.r10086.com:8000/" + type + ".php");
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
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
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
            while ((i = receiveStream.Read(buffer1, 0, buffer1.Length)) > 0)
            {
                stmMemory.Write(buffer1, 0, i);
            }
            //д�����
            string name = System.IO.Path.GetFileName(originalString);
            //name = "upload\\" + name;
            //if (!File.Exists(name))
            //{
            //    FileStream file = new FileStream(name, FileMode.Create, FileAccess.ReadWrite);
            //    file.Write(stmMemory.ToArray());
            //    file.Flush();
            //    file.Close();
            //}
            //FileStream file = new FileStream("1.jpg",FileMode.Create, FileAccess.ReadWrite);
            //�ر���
            receiveStream.Close();
            response.Close();
            key = name;
            return buffer1;
        }


    }
}
