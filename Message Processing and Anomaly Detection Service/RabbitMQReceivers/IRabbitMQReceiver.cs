namespace Message_Processing_and_Anomaly_Detection_Service.RabbitMQReceivers;

public interface IRabbitMQReceiver : IDisposable
{
    Task<string> ReceivePayloadAsync();
    Task StartAsync();
}