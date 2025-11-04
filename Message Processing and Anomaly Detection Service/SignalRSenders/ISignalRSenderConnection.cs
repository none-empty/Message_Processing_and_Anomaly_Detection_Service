namespace Message_Processing_and_Anomaly_Detection_Service.SignalRSenders;

public interface ISignalRSenderConnection
{
    public Task StartAsync();
    public Task InvokeAsync(string remoteProcedure, string message);
}