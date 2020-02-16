using System;

namespace KN.RabbitMQ.Queue.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class QueueConsumer : Attribute
    {
        public string Value { get; set; }

        /// <summary>Initializes a new instance of the <see cref="QueueConsumer"/> class.</summary>
        /// <param name="value">The value.</param>
        public QueueConsumer(string value)
        {
            Value = value;
        }
    }
}
