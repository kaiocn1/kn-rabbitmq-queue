## About

Easy-to-use RabbitMQ Queue library for .NET products.
This is project is derived from [RabbitMQ.Cliente](https://www.rabbitmq.com/dotnet.html) library.

## [Source Code](https://github.com/kaiocn1/kn-rabbitmq-queue.git)




## Installation

Get [KN.RabbitMQ.Queue](https://www.nuget.org/packages/KN.RabbitMQ.Queue) from NuGet.  The purpose of this fork is to add support to easiest .NET Standard development.

```powershell
PM> Install-Package KN.RabbitMQ.Queue
```

## Usage

### Init in Startup

Write in appsettings file your RabbitMQ Broker credentials, ID is the name for the QUEUE.

```json

{
  "KNQueueConfigurations": {
    "HostName": "example.cloudamqp.com",
    "Port": 80,
    "UserName": "example",
    "Password": "example",
    "Uri": "amqp://example:example@example.cloudamqp.com/example",
    "Id": "TestApi"
  }
}

```

Startup.cs
See in examples files for other project files

```csharp
    public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            MessageBroker.Init(services);
        }
```


### Publish message

Publish your message with the "Type" you need in message.

```csharp
 [HttpPost]
        public object Post(
            [FromBody]Message message)
        {
            var queue = new MessageQueue("TestClient");

            MessageBroker.Publish(queue, message);

            return new
            {
                Result = "Message sent",
                Message = message
            };
        }
```

### Consume a message

Implement the interface IMessagingService, with the QueueConsumer attribute is the same name of the Type in message.

```csharp
    [QueueConsumer("TestMessageApi")]
    public class QueueReturnConsumerService : IMessagingService
    {
        public MessageResult Run(Message message, IServiceScopeFactory _scopeFactory)
        {
            Console.WriteLine(JsonConvert.SerializeObject(message));

            return new MessageResult() { Sucess = true };
        }

        public bool UnavaibleService()
        {
            return false;
        }
    }
```

## Credits

- [RabbitMQ](https://www.rabbitmq.com/dotnet.html) for the original work.


## License

This project is licensed under [MIT License](https://github.com/kaiocn1/kn-rabbitmq-queue/blob/master/LICENSE).
