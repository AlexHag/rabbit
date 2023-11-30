namespace Rabbit.Domain.Events;

public class WaterFlowStoppedEvent : EventBase
{
    public static string QueueName = "water-flow-stopped";
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string StreamName { get; set; } = string.Empty;
}