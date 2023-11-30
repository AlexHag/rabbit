using Rabbit.Domain.Events;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Rabbit.Domain.Consumers;
using Rabbit.Domain.Options;

namespace Rabbit.Worker.Consumers;

public class LemonConsumer : ConsumerBase<LemonEvent>, IHostedService
{
    private readonly ILogger<LemonConsumer> _logger;

    public LemonConsumer(
        ILogger<LemonConsumer> logger,
        IOptions<RabbitMQOptions> options)
        : base(LemonEvent.QueueName, options)
    {
        _logger = logger;
    }

    public override Task Consume(LemonEvent message)
    {
        _logger.LogInformation($"Consumed Lemon... Sourness: {message.Sourness}");
        return Task.CompletedTask;
    }

    public override Task HandleJsonException(JsonException ex)
    {
        _logger.LogError($"Failed to deserialize Lemon message: {ex.Message}");
        return Task.CompletedTask;
    }
}
