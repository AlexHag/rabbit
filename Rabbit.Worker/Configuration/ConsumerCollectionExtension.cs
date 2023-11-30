using System.Reflection;
using Rabbit.Worker.Consumers;

namespace Rabbit.Worker.Configuration;

public static class ConsumerCollectionExtension
{
    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.ConfigureOptions<ConsumerOptionsSetup>();
        services.AddHostedService<BananaConsumer>();
        services.AddHostedService<LemonConsumer>();
        services.AddHostedService<PineappleConsumer>();

        return services;
    }
}