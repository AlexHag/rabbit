using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Stream.Client;
using Rabbit.Domain.Events;
using Rabbit.Domain.Consumers;
using Rabbit.Domain.Options;

namespace Rabbit.Worker.StreamConsumers;

public class WaterFlowConsumer : StreamConsumerBase
{
    private readonly ILogger<WaterFlowConsumer> _logger;
    private readonly WaterFlowStartedEvent _waterFlowStartedEvent;

    public WaterFlowConsumer(
        ILogger<WaterFlowConsumer> logger,
        WaterFlowStartedEvent waterFlowStartedEvent,
        IOptions<RabbitMQOptions> options) : base(waterFlowStartedEvent.RiverName, options)
    {
        _waterFlowStartedEvent = waterFlowStartedEvent;
        _logger = logger;
    }

    public override Task Consume(string sourceStream, RawConsumer consumer, MessageContext messageContext, Message message)
    {
        _logger.LogInformation($@"Consumed water stream: {Encoding.ASCII.GetString(message.Data.Contents)}
            SoureceStream: {sourceStream}
            Context Ofset: {messageContext.Offset}
            Context Timestamp: {messageContext.Timestamp}");
        return Task.CompletedTask;
    }

    public override async Task HandleCompletion()
    {
        _logger.LogInformation("WaterConsumer completed.");
        await DisposeAsync();
    }
}