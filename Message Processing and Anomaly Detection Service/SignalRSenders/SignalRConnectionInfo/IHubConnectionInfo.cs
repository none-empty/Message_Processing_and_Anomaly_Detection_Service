namespace Message_Processing_and_Anomaly_Detection_Service.SignalRSenders.SignalRConnectionInfo;

public interface IHubConnectionInfo
{
    public string Url { get; init; }
    public string RemoteProcedureName { get; init; }
}