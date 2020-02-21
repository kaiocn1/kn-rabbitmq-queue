using KN.RabbitMQ.Queue.Attributes;
using KN.RabbitMQ.Queue.Domain;
using KN.RabbitMQ.Queue.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KN.RabbitMQ.Queue
{
    public class ConsumeHostedService : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private QueueConfigurations _configurations;
        private readonly IServiceScopeFactory _scopeFactory;
        public ConsumeHostedService(IServiceScopeFactory scopeFactory)
        {
            InitRabbitMQ();
            _scopeFactory = scopeFactory;
        }

        private void InitRabbitMQ()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json");
            var configuration = builder.Build();
            _configurations = new QueueConfigurations();
            new ConfigureFromConfigurationOptions<QueueConfigurations>(
                configuration.GetSection("KNQueueConfigurations"))
                    .Configure(_configurations);

            var factory = new ConnectionFactory()
            {
                HostName = _configurations.HostName,
                Port = _configurations.Port,
                UserName = _configurations.UserName,
                Password = _configurations.Password,
                Uri = new Uri(_configurations.Uri)
            };

            factory.RequestedHeartbeat = 60;

            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_configurations.Id, false, false, false, null);
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;
            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(_configurations.Id, false, consumer);
            return Task.CompletedTask;
        }

        private void Consumer_Received(
        object sender, BasicDeliverEventArgs e)
        {
            var conteudo = Encoding.UTF8.GetString(e.Body);
            var mensagem = JsonConvert.DeserializeObject<Message>(conteudo);
            var servico = GetService(mensagem);

            if (servico != null)
            {
                Task.Run(() => RunMessage(mensagem, servico, e.DeliveryTag));

                while (servico.UnavailableService())
                {
                    Thread.Sleep(10);
                }
            }
        }

        private void RunMessage(Message mensagem, IMessagingService servico, ulong tag)
        {
            try
            {
                var processeMensagem = Publish(mensagem, servico);

                if (processeMensagem == null || !processeMensagem.Sucess)
                {
                    return;
                }

                if (processeMensagem.ReturnQueue != null)
                {
                    MessageBroker.Publish(_channel, processeMensagem.ReturnQueue, processeMensagem.ReturnMessage, processeMensagem.ReturnQueue.AutoDelete);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _channel.BasicAck(tag, false);
            }
        }

        private MessageResult Publish(Message message, IMessagingService service)
        {
            if (service == null)
            {
                return new MessageResult();
            }

            return service.Run(message, _scopeFactory);
        }

        private IMessagingService GetService(Message message)
        {
            var targetType = AppDomain.CurrentDomain
             .GetAssemblies()
             .Where(x => !x.IsDynamic)
             .SelectMany(x => x.ExportedTypes)
             .FirstOrDefault(x => typeof(IMessagingService).IsAssignableFrom(x)
                 && !x.IsInterface
                 && !x.IsAbstract
                 && x.GetCustomAttribute<QueueConsumer>()?.Value == message.Type);

            if (targetType == null)
            {
                return null;
            }

            return Activator.CreateInstance(targetType) as IMessagingService;
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
