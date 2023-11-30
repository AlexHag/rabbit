using Rabbit.Domain.Events;
using Rabbit.Worker.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Rabbit.Worker.Consumers;

public class LemonConsumer : ConsumerBase<LemonEvent>
{
    private readonly ILogger<LemonConsumer> _logger;

    public LemonConsumer(
        ILogger<LemonConsumer> logger,
        IOptions<ConsumerOptions> options)
        : base(LemonEvent.QueueName, options)
    {
        _logger = logger;
    }

    public override void Consume(LemonEvent message)
    {
        _logger.LogInformation($"Consumed Lemon... Sourness: {message.Sourness}");
    }

    public override void HandleJsonException(JsonException ex)
    {
        _logger.LogError($"Failed to deserialize Lemon message: {ex.Message}");
    }
}
