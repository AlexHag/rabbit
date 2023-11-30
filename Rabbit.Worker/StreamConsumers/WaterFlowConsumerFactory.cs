using Microsoft.Extensions.Options;
using Rabbit.Domain.Events;
using Rabbit.Domain.Options;

namespace Rabbit.Worker.StreamConsumers;

public interface IWaterFlowConsumerFactory
{
    void CreateWaterFlowConsumer(WaterFlowStartedEvent waterFlowStartedEvent, int retryCount = 0);
}

public class WaterFlowConsumerFactory : IWaterFlowConsumerFactory
{
    private readonly ILogger<WaterFlowConsumer> _logger;
    private readonly IOptions<RabbitMQOptions> _options;

    public WaterFlowConsumerFactory(
        ILogger<WaterFlowConsumer> logger,
        IOptions<RabbitMQOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    public async void CreateWaterFlowConsumer(WaterFlowStartedEvent waterFlowStartedEvent, int retryCount = 0)
    {
        try
        {
            var waterFlowConsumer = new WaterFlowConsumer(_logger, waterFlowStartedEvent, _options);
            await waterFlowConsumer.Start();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating water flow consumer, production might not have begun yet, retrying...");
            if (retryCount < 3)
            {
                retryCount++;
                _logger.LogInformation($"Retrying to create water flow consumer. Retry count: {retryCount}");
                await Task.Delay(1000);
                CreateWaterFlowConsumer(waterFlowStartedEvent, retryCount);
            }
            else
            {
                _logger.LogError($"Failed to create water flow consumer after {retryCount} retries");
            }
        }
    }
}
