namespace Rabbit.Domain.Events;

public class PineappleEvent : EventBase
{
    public static string QueueName = "pineapple";
    public string CountryOfOrigin { get; set; }
}
