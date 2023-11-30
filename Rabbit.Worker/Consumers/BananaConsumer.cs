using Rabbit.Domain.Events;
using Rabbit.Worker.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Rabbit.Worker.Consumers;

public class BananaConsumer : ConsumerBase<BananaEvent>
{
    private readonly ILogger<BananaConsumer> _logger;

    public BananaConsumer(
        ILogger<BananaConsumer> logger,
        IOptions<ConsumerOptions> options)
        : base(BananaEvent.QueueName, options)
    {
        _logger = logger;
    }

    public override void Consume(BananaEvent message)
    {
        _logger.LogInformation($"Consumed Banana... Length: {message.Length}, Calories: {message.Calories}");
    }

    public override void HandleJsonException(JsonException ex)
    {
        _logger.LogError($"Failed to deserialize Banana message: {ex.Message}");
    }
}
