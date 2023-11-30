using Microsoft.Extensions.Options;

namespace Rabbit.Service.Configuration;

public class ProducerOptions
{
    public string HostName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ProducerOptionsSetup : IConfigureOptions<ProducerOptions>
{
    private const string SectionName = "RabbitMQOptions";
    private readonly IConfiguration _configuration;

    public ProducerOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(ProducerOptions options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}
