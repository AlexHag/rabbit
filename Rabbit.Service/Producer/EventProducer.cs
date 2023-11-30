using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Rabbit.Service.Configuration;
using Rabbit.Domain.Events;

namespace Rabbit.Service.Producer;

public class EventProducer<T> : IDisposable where T : EventBase
{
    private readonly IModel _channel;
    private readonly IConnection _connection;

    public EventProducer(IOptions<ProducerOptions> options)
    {
        var factory = new ConnectionFactory
        {
            HostName = options.Value.HostName,
            UserName = options.Value.Username,
            Password = options.Value.Password
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Produce(T data)
    {
        var queueName = data.GetType().GetField("QueueName")?.GetValue(null) as string;

        _channel.QueueDeclare(queue: queueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var message = JsonSerializer.Serialize(data);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: string.Empty,
                             routingKey: queueName,
                             basicProperties: null,
                             body: body);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
