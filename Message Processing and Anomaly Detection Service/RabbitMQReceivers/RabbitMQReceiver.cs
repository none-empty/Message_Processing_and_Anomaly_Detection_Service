using System.Text;
using System.Threading.Channels;
using Message_Processing_and_Anomaly_Detection_Service.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Message_Processing_and_Anomaly_Detection_Service.RabbitMQReceivers;

public class RabbitMQReceiver : IRabbitMQReceiver
{
    private const string QueueName = "server_statistics_queue";
    private readonly IConnectionFactory _connectionFactory;
    private readonly SemaphoreSlim _lifecycleLock = new(1, 1);
    private readonly CancellationTokenSource _shutdown = new();
    private readonly Channel<string> _messageBuffer;
    private readonly ushort _prefetchCount;
    private IConnection? _connection;
    private IChannel? _channel;
    private bool _started;
    private bool _disposed;

    public RabbitMQReceiver(IConnectionFactory connectionFactory, IOptions<RabbitMQOptions> options)
    {
        _connectionFactory = connectionFactory;
        _prefetchCount = options.Value.PrefetchCount;
        _messageBuffer = Channel.CreateBounded<string>(
            new BoundedChannelOptions(options.Value.BufferCapacity)
            {
                SingleReader = true,
                SingleWriter = true,
                FullMode = BoundedChannelFullMode.Wait
            });
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _lifecycleLock.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_started)
            {
                return;
            }

            _connection ??= await _connectionFactory.CreateConnectionAsync(cancellationToken);
            var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
            try
            {
                await channel.QueueDeclareAsync(
                    queue: QueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: cancellationToken);

                await channel.BasicQosAsync(
                    prefetchSize: 0,
                    prefetchCount: _prefetchCount,
                    global: false,
                    cancellationToken: cancellationToken);

                var shutdownToken = _shutdown.Token;
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (_, eventArgs) =>
                {
                    // Copy the body before the callback returns and the client's memory is released.
                    var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
                    using var deliveryCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                        shutdownToken, eventArgs.CancellationToken);
                    var deliveryToken = deliveryCancellation.Token;
                    try
                    {
                        await _messageBuffer.Writer.WriteAsync(message, deliveryToken);

                        // Receipt means acceptance into our buffer, independently of later processing.
                        await channel.BasicAckAsync(eventArgs.DeliveryTag, false, deliveryToken);
                    }
                    catch (OperationCanceledException) when (deliveryToken.IsCancellationRequested)
                    {
                        // Unacknowledged deliveries are returned to RabbitMQ when the channel closes.
                    }
                };

                await channel.BasicConsumeAsync(
                    queue: QueueName,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: cancellationToken);

                _channel = channel;
                _started = true;
            }
            catch
            {
                await channel.DisposeAsync();
                throw;
            }
        }
        finally
        {
            _lifecycleLock.Release();
        }
    }

    public async Task<string> ReceivePayloadAsync(CancellationToken cancellationToken = default)
    {
        return await _messageBuffer.Reader.ReadAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _lifecycleLock.WaitAsync();
        try
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            await _shutdown.CancelAsync();
            try
            {
                if (_channel is not null)
                {
                    await _channel.DisposeAsync();
                }
            }
            finally
            {
                try
                {
                    if (_connection is not null)
                    {
                        await _connection.DisposeAsync();
                    }
                }
                finally
                {
                    _messageBuffer.Writer.TryComplete();
                    _shutdown.Dispose();
                }
            }
        }
        finally
        {
            _lifecycleLock.Release();
        }
    }
}
