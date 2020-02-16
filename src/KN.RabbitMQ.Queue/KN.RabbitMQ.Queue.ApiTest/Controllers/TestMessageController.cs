using KN.RabbitMQ.Queue.Domain;
using Microsoft.AspNetCore.Mvc;

namespace KN.RabbitMQ.Queue.ApiTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestMessageController : ControllerBase
    {
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
    }
}
