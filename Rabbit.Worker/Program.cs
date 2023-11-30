using Rabbit.Worker.Configuration;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging();
                services.AddConsumers();
            })
            .Build();
        
        await host.RunAsync();
    }
}
