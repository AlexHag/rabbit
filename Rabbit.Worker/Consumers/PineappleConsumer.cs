using Rabbit.Domain.Events;
using Rabbit.Worker.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Rabbit.Worker.Consumers;

public class PineappleConsumer : ConsumerBase<PineappleEvent>
{
    private readonly ILogger<PineappleConsumer> _logger;

    public PineappleConsumer(
        ILogger<PineappleConsumer> logger,
        IOptions<ConsumerOptions> options)
        : base(PineappleEvent.QueueName, options)
    {
        _logger = logger;
    }

    public override void Consume(PineappleEvent message)
    {
        _logger.LogInformation($"Consumed Pineapple... Country of origin: {message.CountryOfOrigin}");
    }

    public override void HandleJsonException(JsonException ex)
    {
        _logger.LogError($"Failed to deserialize Pineapple message: {ex.Message}");
    }
}
