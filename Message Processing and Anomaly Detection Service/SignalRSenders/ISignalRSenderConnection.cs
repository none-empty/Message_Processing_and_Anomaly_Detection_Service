namespace Message_Processing_and_Anomaly_Detection_Service.SignalRSenders;

public interface ISignalRSenderConnection
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task SendAsync(string message, CancellationToken cancellationToken = default);
}
