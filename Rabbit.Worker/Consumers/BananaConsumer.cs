using Rabbit.Domain.Events;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Rabbit.Domain.Consumers;
using Rabbit.Domain.Options;

namespace Rabbit.Worker.Consumers;

public class BananaConsumer : ConsumerBase<BananaEvent>, IHostedService
{
    private readonly ILogger<BananaConsumer> _logger;

    public BananaConsumer(
        ILogger<BananaConsumer> logger,
        IOptions<RabbitMQOptions> options)
        : base(BananaEvent.QueueName, options)
    {
        _logger = logger;
    }

    public override Task Consume(BananaEvent message)
    {
        _logger.LogInformation($"Consumed Banana... Length: {message.Length}, Calories: {message.Calories}");
        return Task.CompletedTask;
    }

    public override Task HandleJsonException(JsonException ex)
    {
        _logger.LogError($"Failed to deserialize Banana message: {ex.Message}");
        return Task.CompletedTask;
    }
}
