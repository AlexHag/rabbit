namespace Rabbit.Domain.Events;

public class WaterFlowStartedEvent : EventBase
{
    public const string QueueName = "water-flow-started";
    public string RiverName { get; set; }
}
