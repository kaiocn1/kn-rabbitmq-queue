using KN.RabbitMQ.Queue.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Text;

namespace KN.RabbitMQ.Queue
{
    /// <summary>Message broker</summary>
    public class MessageBroker
    {
        public static QueueConfigurations _configurations;
        private static IConfiguration _configuration;
        private static IConnection _connection;

        /// <summary>Initializes the message broker.</summary>
        /// <param name="services">The services.</param>
        public static void Init(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json");
            _configuration = builder.Build();

            var rabbitMQConfigurations = new QueueConfigurations();
            new ConfigureFromConfigurationOptions<QueueConfigurations>(
                _configuration.GetSection("KNQueueConfigurations"))
                    .Configure(rabbitMQConfigurations);

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(rabbitMQConfigurations.Uri)
            };

            _connection = factory.CreateConnection();

            services.AddHostedService<ConsumeHostedService>();
        }

        /// <summary>Publishes the specified message.</summary>
        /// <param name="queue">The queue.</param>
        /// <param name="message">The message.</param>
        public static void Publish(MessageQueue queue, Message message)
        {
            using (var channel = _connection.CreateModel())
            {
                Publish(channel, queue, message);
            }
        }

        /// <summary>Publishes the specified message.</summary>
        /// <param name="channel">The channel.</param>
        /// <param name="queue">The queue.</param>
        /// <param name="message">The message.</param>
        public static void Publish(IModel channel, MessageQueue queue, Message message)
        {
            Publish(channel, queue, message, queue.AutoDelete);
        }

        /// <summary>Publishes the specified message.</summary>
        /// <param name="channel">The channel.</param>
        /// <param name="queue">The queue.</param>
        /// <param name="message">The message.</param>
        /// <param name="autoDelete">if set to <c>true</c> automatic delete.</param>
        public static void Publish(IModel channel, MessageQueue queue, Message message, bool autoDelete)
        {
            lock (channel)
            {
                channel.QueueDeclare(queue: queue.Name,
                                 durable: queue.Durable,
                                 exclusive: queue.Exclusive,
                                 autoDelete: autoDelete,
                                 arguments: queue.Arguments);


                var conteudo = JsonConvert.SerializeObject(message);

                var body = Encoding.UTF8.GetBytes(conteudo);
                channel.BasicPublish(exchange: "",
                                     routingKey: queue.Name,
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
