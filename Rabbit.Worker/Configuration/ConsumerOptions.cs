using Microsoft.Extensions.Options;

namespace Rabbit.Worker.Configuration;

public class ConsumerOptions
{
    public string HostName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ConsumerOptionsSetup : IConfigureOptions<ConsumerOptions>
{
    private const string SectionName = "RabbitMQOptions";
    private readonly IConfiguration _configuration;

    public ConsumerOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(ConsumerOptions options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}
