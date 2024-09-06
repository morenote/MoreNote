
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MoreNote.Config.ConfigFile;
using MoreNote.Logic.Service;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MoreNoteWorkerService
{
	/// <summary>
	/// VuePress��̨��������
	/// </summary>
	public class VuePressWorker : BackgroundService
	{
		private readonly ILogger<VuePressWorker> _logger;
		private ConfigFileService configFileService;
		/// <summary>
		/// ��վ����
		/// </summary>
		static WebSiteConfig config;
		public VuePressWorker()
		{

		}

		Random random = new Random();

		public VuePressWorker(ILogger<VuePressWorker> logger, ConfigFileService configFileService)
		{
			_logger = logger;
			this.configFileService = configFileService;
			config = configFileService.ReadConfig();
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
