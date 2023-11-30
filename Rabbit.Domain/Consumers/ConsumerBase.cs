using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using Rabbit.Domain.Events;
using Rabbit.Domain.Options;

namespace Rabbit.Domain.Consumers;

public abstract class ConsumerBase<T> : IDisposable
    where T : EventBase
{
    private readonly string _queueName;
    private readonly IModel _channel;
    private readonly IConnection _connection;

    public ConsumerBase(
        string queueName,
        IOptions<RabbitMQOptions> options)
    {
        _queueName = queueName;
        var factory = new ConnectionFactory
        {
            HostName = options.Value.HostName,
            UserName = options.Value.Username,
            Password = options.Value.Password
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public abstract Task Consume(T data);
    public abstract Task HandleJsonException(JsonException ex);

    private void ConsumeAndSerialize(object? model, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        try
        {
            var data = JsonSerializer.Deserialize<T>(message);
            Consume(data!);
        }
        catch (JsonException ex)
        {
            HandleJsonException(ex);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _channel.QueueDeclare(queue: _queueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += ConsumeAndSerialize;
        _channel.BasicConsume(queue: _queueName,
                             autoAck: true,
                             consumer: consumer);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        _connection?.Close();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}