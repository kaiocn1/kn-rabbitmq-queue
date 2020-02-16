using KN.RabbitMQ.Queue.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace KN.RabbitMQ.Queue.Interfaces
{
    public interface IMessagingService
    {
        MessageResult Run(Message message, IServiceScopeFactory _scopeFactory);
        bool UnavaibleService();
    }
}
