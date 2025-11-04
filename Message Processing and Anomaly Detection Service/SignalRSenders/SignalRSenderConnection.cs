using Message_Processing_and_Anomaly_Detection_Service.SignalRSenders.SignalRConnectionInfo;
using Microsoft.AspNetCore.SignalR.Client;

namespace Message_Processing_and_Anomaly_Detection_Service.SignalRSenders;

public class SignalRSenderConnection : ISignalRSenderConnection
{
    private readonly HubConnection _connection;
    
    public SignalRSenderConnection(IHubConnectionInfo connectionInfo)
    {
         _connection = new HubConnectionBuilder()
            .WithUrl(connectionInfo.Url)
            .Build();
    }


    public Task StartAsync()
    {
        return _connection.StartAsync();
    }

    public Task InvokeAsync(string remoteProcedure, string message)
    {
        return _connection.InvokeAsync(remoteProcedure, message);
    }
}