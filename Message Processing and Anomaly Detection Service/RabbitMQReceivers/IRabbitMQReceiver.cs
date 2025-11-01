namespace Message_Processing_and_Anomaly_Detection_Service.RabbitMQReceivers;

public interface IRabbitMQReceiver
{
    public Task<string> ReceivePayload();
}