namespace Message_Processing_and_Anomaly_Detection_Service.RabbitMQReceivers;

public interface IRabbitMQReceiver : IAsyncDisposable
{
    Task<string> ReceivePayloadAsync(CancellationToken cancellationToken = default);
    Task StartAsync(CancellationToken cancellationToken = default);
}
