using Rabbit.Domain.Events;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Rabbit.Domain.Consumers;
using Rabbit.Domain.Options;

namespace Rabbit.Worker.Consumersss;

public class PineappleConsumer : ConsumerBase<PineappleEvent>, IHostedService
{
    private readonly ILogger<PineappleConsumer> _logger;

    public PineappleConsumer(
        ILogger<PineappleConsumer> logger,
        IOptions<RabbitMQOptions> options)
        : base(PineappleEvent.QueueName, options)
    {
        _logger = logger;
    }

    public override Task Consume(PineappleEvent message)
    {
        _logger.LogInformation($"Consumed Pineapple... Country of origin: {message.CountryOfOrigin}");
        return Task.CompletedTask;
    }

    public override Task HandleJsonException(JsonException ex)
    {
        _logger.LogError($"Failed to deserialize Pineapple message: {ex.Message}");
        return Task.CompletedTask;
    }
}
