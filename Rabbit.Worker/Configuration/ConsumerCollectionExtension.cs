using Rabbit.Worker.Consumers;
using Rabbit.Worker.Consumersss;
using Rabbit.Worker.StreamConsumers;
using Rabbit.Domain.Options;

namespace Rabbit.Worker.Configuration;

public static class ConsumerCollectionExtension
{
    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.ConfigureOptions<RabbitMQOptionsSetup>();
        services.AddSingleton<IWaterFlowConsumerFactory, WaterFlowConsumerFactory>();

        services.AddHostedService<BananaConsumer>();
        services.AddHostedService<LemonConsumer>();
        services.AddHostedService<PineappleConsumer>();
        services.AddHostedService<WaterFlowStartedConsumer>();

        services.BuildServiceProvider().GetRequiredService<IWaterFlowConsumerFactory>().CreateWaterFlowConsumer(new Domain.Events.WaterFlowStartedEvent{ RiverName = "water-flow"});

        return services;
    }
}