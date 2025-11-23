using Message_Processing_and_Anomaly_Detection_Service.SignalRSenders.SignalRConnectionInfo;
using Microsoft.AspNetCore.SignalR.Client;

namespace Message_Processing_and_Anomaly_Detection_Service.SignalRSenders;

public class SignalRSenderConnection : ISignalRSenderConnection
{
    private readonly HubConnection _connection;
    private readonly string _remoteProcedureName;
    public SignalRSenderConnection(IHubConnectionInfo connectionInfo)
    {
         _connection = new HubConnectionBuilder()
            .WithUrl(connectionInfo.Url)
            .Build();

         _remoteProcedureName = connectionInfo.RemoteProcedureName;
    }


    public Task StartAsync()
    {
        return _connection.StartAsync();
    }

    public Task SendAsync(string message)
    {
        return _connection.InvokeAsync(_remoteProcedureName, message);
    }
}