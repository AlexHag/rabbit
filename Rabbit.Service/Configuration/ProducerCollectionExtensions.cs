using System.Reflection;
using Rabbit.Domain.Events;
using Rabbit.Domain.Producers;

namespace Rabbit.Service.Configuration;

public static class ProducerCollectionExtensions
{
    public static RouteGroupBuilder MapProducerEndpoints(this RouteGroupBuilder group)
    {
        var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(EventBase).IsAssignableFrom(p));

        foreach (var eventType in eventTypes)
        {
            if (eventType == typeof(EventBase))
            {
                continue;
            }

            var queueName = eventType.GetField("QueueName")?.GetValue(null) as string;
            if (String.IsNullOrWhiteSpace(queueName))
            {
                throw new Exception($"Event {eventType.Name} does not have a QueueName");
            }

            var method = typeof(ProducerCollectionExtensions).GetMethod(nameof(ProduceEvent), BindingFlags.NonPublic | BindingFlags.Static)!;
            
            var generic = method.MakeGenericMethod(eventType);
            var delegateType = typeof(Func<,,>).MakeGenericType(eventType, typeof(EventProducer<>).MakeGenericType(eventType), typeof(IResult));
            var handler = Delegate.CreateDelegate(delegateType, generic);
            
            group.MapPost($"{queueName}", handler);
        }
        return group;
    }

    private static IResult ProduceEvent<T>(T data, EventProducer<T> producer) where T : EventBase
    {
        producer.Produce(data);
        return Results.Ok();
    }
}
