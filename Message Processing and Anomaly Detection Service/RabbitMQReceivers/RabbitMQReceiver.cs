using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Message_Processing_and_Anomaly_Detection_Service.RabbitMQReceivers;

public class RabbitMQReceiver : IRabbitMQReceiver, IAsyncDisposable
{
    private readonly IConnection _connection;
    private IChannel _channel;
    private readonly Channel<string> _messageBuffer;
    private const string QueueName = "server_statistics_queue";
    public RabbitMQReceiver(IConnection connection)
    {
        this._connection = connection;

        _messageBuffer = Channel.CreateUnbounded<string>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = true
            });
       
    }

    public async Task StartAsync()
    {
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            await _messageBuffer.Writer.WriteAsync(message);

            await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
        };

        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer);
    }
    
    public async Task<string> ReceivePayloadAsync()
    {
        return await _messageBuffer.Reader.ReadAsync();
    }

    public void Dispose()
    {
        _connection.Dispose();
        _channel.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _channel.DisposeAsync();
    }
}