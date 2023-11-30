using System.Net;
using Rabbit.Domain.Producers;
using Rabbit.Domain.Events;
using Rabbit.Domain.Options;
using Microsoft.Extensions.Options;

namespace Rabbit.ConsoleApp;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var options = new RabbitMQOptions
        {
            HostName = "localhost",
            Username = "admin",
            Password = "password",
            Endpoints = new List<EndPoint>() { new IPEndPoint(IPAddress.Loopback, 5552) }
        };

        var streamProdcer = await StreamProducer.CreateStreamProducer(
            "water-flow",
            options
        );

        var waterFlowStartedEventProducer = new EventProducer<WaterFlowStartedEvent>(Options.Create(options));
        waterFlowStartedEventProducer.Produce(new WaterFlowStartedEvent {RiverName = "water-flow"});

        var fileStreamProducer = new FileStreamProducer(streamProdcer);
        await fileStreamProducer.StartReading();
    }
}
