namespace Rabbit.Domain.Events;

public class LemonEvent : EventBase
{
    public static string QueueName = "lemon";
    public int Sourness { get; set; }
}
