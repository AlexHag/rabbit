namespace Rabbit.Domain.Events;

public class BananaEvent : EventBase
{
    public static string QueueName = "banana";
    public int Length { get; set; }
    public int Calories { get; set; }
}
