using KN.RabbitMQ.Queue.Attributes;
using KN.RabbitMQ.Queue.Domain;
using KN.RabbitMQ.Queue.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace KN.RabbitMQ.Queue.ConsoleClienteTest
{
    [QueueConsumer("TestMessageApi")]
    public class TestApiService : IMessagingService
    {
        public MessageResult Run(Message message, IServiceScopeFactory _scopeFactory)
        {
            Console.WriteLine(JsonConvert.SerializeObject(message));

            var returnMessage = new MessageResult()
            {
                Sucess = true,
                ReturnQueue = new MessageQueue("TestApi"),
                ReturnMessage = new Message()
                {
                    Type = "TestMessageApi",
                    Text = "Valor"
                }
            };

            return returnMessage;
        }

        public bool UnavaibleService()
        {
            return false;
        }
    }
}
