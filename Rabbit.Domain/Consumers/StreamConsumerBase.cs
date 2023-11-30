using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;
using Rabbit.Domain.Options;

namespace Rabbit.Domain.Consumers;

public abstract class StreamConsumerBase : IAsyncDisposable
{
    protected readonly StreamSystem _streamSystem;
    protected readonly Consumer _consumer;
    protected readonly string _streamName;
    protected TaskCompletionSource<int> consumerTaskCompletionSource = new TaskCompletionSource<int>();
    private int consumedCount = 0;

    public StreamConsumerBase(string streamName, IOptions<RabbitMQOptions> options)
    {
        _streamName = streamName;

        _streamSystem = StreamSystem.Create(
            new StreamSystemConfig()
            {
                UserName = options.Value.Username,
                Password = options.Value.Password,
                Endpoints = options.Value.Endpoints
            }
        ).ConfigureAwait(false).GetAwaiter().GetResult();

        _consumer = Consumer.Create(
                new ConsumerConfig(_streamSystem, streamName)
                {
                    OffsetSpec = new OffsetTypeFirst(),
                    MessageHandler = async (sourceStream, consumer, messageContext, message) => // (3)
                    {
                        Interlocked.Increment(ref consumedCount);
                        if (Encoding.ASCII.GetString(message.Data.Contents) == "STEAM_ENDED")
                        {
                            consumerTaskCompletionSource.SetResult(consumedCount);
                        }
                        await Consume(sourceStream, consumer, messageContext, message);
                    }
                }
            )
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public virtual async Task Start()
    {
        await consumerTaskCompletionSource.Task.ConfigureAwait(false);
        await HandleCompletion();
    }

    public abstract Task Consume(string sourceStream, RawConsumer consumer, MessageContext messageContext, Message message);
    public abstract Task HandleCompletion();

    public async ValueTask DisposeAsync()
    {
        await _consumer.Close().ConfigureAwait(false);
        await _streamSystem.DeleteStream(_streamName).ConfigureAwait(false);
        await _streamSystem.Close().ConfigureAwait(false);
    }
}