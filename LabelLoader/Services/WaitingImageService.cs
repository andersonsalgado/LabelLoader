﻿using GeekBurger.LabelLoader.Contract;
using GeekBurger.LabelLoader.Migrations;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
        public WaitingImageService(
                ILogger<WaitingImageService> logger, 
                IConfiguration configuration,
                ILoggerFactory loggerFactory)
        {
            _logger = logger;
            Configuration = configuration;
            _labelContext = new LabelContext(configuration);
            _loggerFactory = loggerFactory;
        }


        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço Iniciado.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(10000));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            DirectoryInfo _dirNotRead = new DirectoryInfo($"{Environment.CurrentDirectory}{Configuration.GetSection("Files")["NotRead"]}");
            FileInfo[] _filesNotRead = _dirNotRead.GetFiles();

            if (_filesNotRead.Length != 0)
            {
                VisionServices _vision = new VisionServices(Configuration, _loggerFactory.CreateLogger<VisionServices>(), _labelContext);
                foreach (var file in _filesNotRead)
                {
                    var imagemString = await _vision.ObterIngredientes(file.FullName);
                    //MainAsync(imagemString).GetAwaiter().GetResult();
                    //_logger.LogInformation("LabelImage: {imagemString}", imagemString);
                }
            }
            
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço Finalizado.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async Task MainAsync(LabelImageAdded imagemString)
        {
            try
            {
                await managementClient.GetTopicAsync(TopicName);
            }
            catch (MessagingEntityNotFoundException)
            {
                await managementClient.CreateTopicAsync(new TopicDescription(TopicName) { EnablePartitioning = true });
            }
            topicClient = new TopicClient(Configuration.GetSection("AzureServiceBusConfig")["connectionString"], TopicName);

            Console.WriteLine("=======================================================");
            Console.WriteLine("Press ENTER key to exit after sending all the messages.");
            Console.WriteLine("=======================================================");

            await SendMessagesAsync(imagemString);

            Console.ReadKey();

            await topicClient.CloseAsync();
        }
        private async Task SendMessagesAsync(LabelImageAdded imagemString)
        {
            try
            {
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bf.Serialize(ms, imagemString);
                        var message = new Message(ms.ToArray());

                        await topicClient.SendAsync(message);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
