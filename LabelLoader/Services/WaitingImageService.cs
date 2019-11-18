using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.Migrations;
using GeekBurger.LabelLoader.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.Services
{
    public class WaitingImageService : IHostedService, IDisposable
    {
        public IConfiguration Configuration { get; }

        private readonly LabelContext _labelContext;
        private readonly ILoggerFactory _loggerFactory;
        const string TopicName = "labelloader";
        private int executionCount = 0;
        private readonly ILogger<WaitingImageService> _logger;
        private Timer _timer;
        private ITopicClient topicClient;
        private ManagementClient managementClient;
        private readonly StringBuilder _logs;

        public WaitingImageService(
                ILogger<WaitingImageService> logger,
                IConfiguration configuration,
                ILoggerFactory loggerFactory)
        {
            _logs = new StringBuilder();

            _logger = logger;
            Configuration = configuration;
            _labelContext = new LabelContext(configuration);
            _loggerFactory = loggerFactory;
        }


        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logs.AppendLine("Serviço Iniciado.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(10000));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            DirectoryInfo _dirNotRead = new DirectoryInfo($"{Environment.CurrentDirectory}{Configuration.GetSection("Files")["NotRead"]}");
            FileInfo[] _filesNotRead = _dirNotRead.GetFiles();

            if (_filesNotRead.Length != 0)
            {
                VisionServices _vision = new VisionServices(Configuration, _loggerFactory.CreateLogger<VisionServices>(), _labelContext);
                foreach (var file in _filesNotRead)
                {
                    var imagemString = _vision.ObterIngredientes(file.FullName).Result;
                    MainAsync(imagemString).GetAwaiter().GetResult();
                    _logs.AppendLine("LabelImage: "+ imagemString);
                }
            }

        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logs.AppendLine("Serviço Finalizado.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async Task MainAsync(LabelImageAdded imagemString)
        {
            try
            {
                managementClient = new ManagementClient(Configuration.GetSection("AzureServiceBusConfig")["connectionString"]);
                await managementClient.GetTopicAsync(TopicName);
            }
            catch (MessagingEntityNotFoundException)
            {
                await managementClient.CreateTopicAsync(new TopicDescription(TopicName) { EnablePartitioning = true });
            }
            topicClient = new TopicClient(Configuration.GetSection("AzureServiceBusConfig")["connectionString"], TopicName);

            await SendMessagesAsync(imagemString);
            await topicClient.CloseAsync();

            var notRead = $"{Environment.CurrentDirectory}{Configuration.GetSection("Files")["NotRead"]}";
            var read = $"{Environment.CurrentDirectory}{Configuration.GetSection("Files")["Read"]}";

            if (!Directory.Exists(notRead))
                Directory.CreateDirectory(notRead);

            if (!Directory.Exists(read))
                Directory.CreateDirectory(read);

            Directory.Move(notRead, read);
        }
        private async Task SendMessagesAsync(LabelImageAdded imagemString)
        {
            try
            {
                {
                    var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(imagemString)));

                    await topicClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                _logs.AppendLine($"{DateTime.Now} :: Exception: {exception.Message}");

            }
            finally
            {
                _logger.LogInformation(_logs.ToString());

                Log _log = new Log();
                _log.DataHora = DateTime.Now;
                _log.Mensagem = _logs.ToString();

                LogService _logService = new LogService();
                await _logService.MainAsync(_log);
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
