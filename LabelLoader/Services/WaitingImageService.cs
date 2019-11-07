using GeekBurger.LabelLoader.Migrations;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
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

        private void DoWork(object state)
        {
            DirectoryInfo _dirNotRead = new DirectoryInfo($"{Environment.CurrentDirectory}{Configuration.GetSection("Files")["NotRead"]}");
            FileInfo[] _filesNotRead = _dirNotRead.GetFiles();

            if (_filesNotRead.Length != 0)
            {
                VisionServices _vision = new VisionServices(Configuration, _loggerFactory.CreateLogger<VisionServices>(), _labelContext);
                foreach (var file in _filesNotRead)
                {
                    _vision.ObterIngredientes(file.FullName);
                    //MainAsync().GetAwaiter().GetResult();  MANDA A MENSAGEM
                }
            }
            
            _logger.LogInformation("Count: {Count}", executionCount++);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço Finalizado.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async Task MainAsync()
        {
            const int numberOfMessages = 10;
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

            // Send messages.
            await SendMessagesAsync(numberOfMessages);

            Console.ReadKey();

            await topicClient.CloseAsync();
        }
        private async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    // Create a new message to send to the topic.
                    string messageBody = $"Message {i}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    // Write the body of the message to the console.
                    Console.WriteLine($"Sending message: {messageBody}");

                    // Send the message to the topic.
                    await topicClient.SendAsync(message);
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
