namespace KN.RabbitMQ.Queue.Domain
{
    public class Message
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public object Data { get; set; }
        public string ReturnQueue { get; set; }
    }
}
