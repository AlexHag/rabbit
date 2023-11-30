using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Rabbit.Domain.Options;

public class RabbitMQOptions
{
    public string HostName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public List<EndPoint> Endpoints { get; set; } = new List<EndPoint>() { new IPEndPoint(IPAddress.Loopback, 5552) };
}

public class RabbitMQOptionsSetup : IConfigureOptions<RabbitMQOptions>
{
    private const string SectionName = "RabbitMQOptions";
    private readonly IConfiguration _configuration;

    public RabbitMQOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(RabbitMQOptions options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}
