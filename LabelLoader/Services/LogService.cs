using GeekBurger.LabelLoader.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.Services
{
    public class LogService
    {
        public IConfiguration Configuration { get; }

        const string TopicName = "MUDAR-AQUI-PARA-O-TOPIC-CORRETO";
        private ITopicClient topicClient;
        private ManagementClient managementClient;

        public async Task MainAsync(Log log)
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

            await SendMessagesAsync(log);
            await topicClient.CloseAsync();
        }
        private async Task SendMessagesAsync(Log log)
        {
            try
            {
                {
                    var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(log)));

                    await topicClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
