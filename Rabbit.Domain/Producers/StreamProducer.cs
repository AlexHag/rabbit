using System.Net;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;
using Rabbit.Domain.Events;
using Microsoft.Extensions.Logging;
using System.Text;
using Rabbit.Domain.Options;

namespace Rabbit.Domain.Producers;

public class StreamProducer : IDisposable
{
    public static async Task<StreamProducer> CreateStreamProducer(string streamName, RabbitMQOptions options)
    {
        var streamSystem = await StreamSystem.Create(
            new StreamSystemConfig()
            {
                UserName = options.Username,
                Password = options.Password,
                Endpoints = options.Endpoints
            }
        ).ConfigureAwait(false);

        await streamSystem.CreateStream(
            new StreamSpec(streamName)
            {
                MaxSegmentSizeBytes = 20_000_000
            }).ConfigureAwait(false);
        
        var producer = await Producer.Create(
            new ProducerConfig(streamSystem, streamName)
        ).ConfigureAwait(false);

        return new StreamProducer(streamSystem, producer);
    }

    public static async void StartWaterFlow(ILogger logger, WaterFlowStartedEvent waterFlowStartedEvent, RabbitMQOptions options)
    {
        var waterFlowProducer = await StreamProducer.CreateStreamProducer(
            waterFlowStartedEvent.RiverName,
            options
        );

        for (var i = 0; i < 100; i++)
        {
            await Task.Delay(100);
            if (i == 50)
            {
                await Task.Delay(2000);
            }
            var message = $"Message {i} : Name: {waterFlowStartedEvent.RiverName}";
            await waterFlowProducer.Send(Encoding.UTF8.GetBytes(message));
            logger.LogInformation($"Sending water: {message}");
        }
        await waterFlowProducer.Send(Encoding.UTF8.GetBytes("STEAM_ENDED"));
    }

    private readonly StreamSystem _streamSystem;
    private readonly Producer _producer;

    public StreamProducer(StreamSystem streamSystem, Producer producer)
    {
        _streamSystem = streamSystem;
        _producer = producer;
    }

    public async Task Send(byte[] data) =>
        await _producer.Send(new Message(data)).ConfigureAwait(false);

    public async void Dispose()
    {
        await _producer.Close().ConfigureAwait(false);
    }
}