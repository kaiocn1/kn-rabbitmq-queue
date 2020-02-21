using KN.RabbitMQ.Queue.Attributes;
using KN.RabbitMQ.Queue.Domain;
using KN.RabbitMQ.Queue.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace KN.RabbitMQ.Queue.ApiTest
{
    [QueueConsumer("TestMessageApi")]
    public class QueueReturnConsumerService : IMessagingService
    {
        public MessageResult Run(Message message, IServiceScopeFactory _scopeFactory)
        {
            Console.WriteLine(JsonConvert.SerializeObject(message));

            return new MessageResult() { Sucess = true };
        }

        public bool UnavailableService()
        {
            return false;
        }
    }
}
