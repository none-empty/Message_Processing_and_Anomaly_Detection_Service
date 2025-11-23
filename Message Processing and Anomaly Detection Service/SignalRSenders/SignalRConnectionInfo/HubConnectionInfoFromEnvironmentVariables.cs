namespace Message_Processing_and_Anomaly_Detection_Service.SignalRSenders.SignalRConnectionInfo;

public class HubConnectionInfoFromEnvironmentVariables : IHubConnectionInfo
{
    public string Url { get; init; }
    public string RemoteProcedureName { get; init; }

    public HubConnectionInfoFromEnvironmentVariables()
    {
        Url = Environment.GetEnvironmentVariable("SignalRUrl")!;
        RemoteProcedureName = Environment.GetEnvironmentVariable("RemoteProcedureName")!;
    }
}