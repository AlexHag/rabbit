using Microsoft.AspNetCore.Mvc;
using Rabbit.Domain.Producers;
using Rabbit.Domain.Events;
using Rabbit.Domain.Options;
using Microsoft.Extensions.Options;

namespace Rabbit.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WaterStreamController : ControllerBase
{
    private readonly ILogger<WaterStreamController> _logger;
    private readonly IOptions<RabbitMQOptions> _options;
    private readonly EventProducer<WaterFlowStartedEvent> _waterFlowStartedEventProducer;

    public WaterStreamController(
        ILogger<WaterStreamController> logger,
        IOptions<RabbitMQOptions> options,
        EventProducer<WaterFlowStartedEvent> waterFlowStartedEventProducer)
    {
        _logger = logger;
        _options = options;
        _waterFlowStartedEventProducer = waterFlowStartedEventProducer;
    }

    [HttpPost]
    public IActionResult ProduceWater(WaterFlowStartedEvent waterFlowStartedEvent)
    {
        _waterFlowStartedEventProducer.Produce(waterFlowStartedEvent);
        StreamProducer.StartWaterFlow(_logger, waterFlowStartedEvent, _options.Value);
        return Ok();
    }
}
