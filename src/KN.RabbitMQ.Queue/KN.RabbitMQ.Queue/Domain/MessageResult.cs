namespace KN.RabbitMQ.Queue.Domain
{
    public class MessageResult
    {
        public bool Sucess { get; set; }
        public Message ReturnMessage { get; set; }
        public MessageQueue ReturnQueue { get; set; }
    }
}
