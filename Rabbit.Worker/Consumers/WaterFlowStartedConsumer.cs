using System.Text.Json;
using Microsoft.Extensions.Options;
using Rabbit.Domain.Consumers;
using Rabbit.Domain.Events;
using Rabbit.Domain.Options;
using Rabbit.Worker.StreamConsumers;

namespace Rabbit.Worker.Consumers;

public class WaterFlowStartedConsumer : ConsumerBase<WaterFlowStartedEvent>, IHostedService
{
    private readonly ILogger<WaterFlowStartedConsumer> _logger;
    private readonly IOptions<RabbitMQOptions> _options;
    private readonly IWaterFlowConsumerFactory _waterFlowConsumerFactory;

    public WaterFlowStartedConsumer(
        ILogger<WaterFlowStartedConsumer> logger,
        IOptions<RabbitMQOptions> options,
        IWaterFlowConsumerFactory streamConsumerHandler)
        : base(WaterFlowStartedEvent.QueueName, options)
    {
        _logger = logger;
        _options = options;
        _waterFlowConsumerFactory = streamConsumerHandler;
    }

    public override Task Consume(WaterFlowStartedEvent message)
    {
        _logger.LogInformation($"Consumed waterflow started event... RiverName: {message.RiverName}");
        _waterFlowConsumerFactory.CreateWaterFlowConsumer(message);
        return Task.CompletedTask;
    }

    public override Task HandleJsonException(JsonException ex)
    {
        throw new NotImplementedException();
    }
}