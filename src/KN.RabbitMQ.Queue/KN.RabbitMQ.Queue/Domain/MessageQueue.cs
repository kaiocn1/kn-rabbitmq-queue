using System.Collections.Generic;

namespace KN.RabbitMQ.Queue.Domain
{
    public class MessageQueue
    {
        public MessageQueue(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
    }
}
