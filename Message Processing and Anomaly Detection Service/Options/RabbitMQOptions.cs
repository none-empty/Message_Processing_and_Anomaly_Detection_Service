namespace Message_Processing_and_Anomaly_Detection_Service.Options;

public class RabbitMQOptions
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public int BufferCapacity { get; set; } = 256;
    public ushort PrefetchCount { get; set; } = 32;
}
