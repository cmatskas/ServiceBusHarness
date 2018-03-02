using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using static System.Console;

namespace SBfeeder
{
    class Program
    {

        private static IConfigurationRoot Configuration;
        const string ConnectionSecretName = "ServiceBusConnectionString";
        const string TopicName = "topic1";
        static TopicClient topicClient;

        public static async Task Main()
        {
            BootstrapConfiguration();
            const int numberOfMessages = 10;
            topicClient = new TopicClient(Configuration[ConnectionSecretName], TopicName);
            await SendMessagesAsync(numberOfMessages);

            WriteLine("Press Enter to exit...");
            ReadKey();

            await topicClient.CloseAsync();
        }

        private static void BootstrapConfiguration()
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (string.IsNullOrWhiteSpace(env))
            {
                env = "Development";
            }

            var builder = new ConfigurationBuilder();

            if (env == "Development")
            {
                builder.AddUserSecrets<Program>();
            }

            Configuration = builder.Build();
        }

        private static async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    string messageBody = $"{{ \"Message\": \"Value of Message {i}\"}}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    WriteLine($"Sending message: {messageBody}");

                    await topicClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
