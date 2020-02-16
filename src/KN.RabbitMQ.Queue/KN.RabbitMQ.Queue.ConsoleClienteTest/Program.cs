using Microsoft.Extensions.Hosting;
using System;

namespace KN.RabbitMQ.Queue.ConsoleClienteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    MessageBroker.Init(services);
                })
                .RunConsoleAsync();

            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }
    }
}
