using Message_Processing_and_Anomaly_Detection_Service.SignalRSenders.SignalRConnectionInfo;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace Message_Processing_and_Anomaly_Detection_Service.SignalRSenders;

public class SignalRSenderConnection : ISignalRSenderConnection, IAsyncDisposable
{
    private readonly HubConnection _connection;
    private readonly string _remoteProcedureName;

    public SignalRSenderConnection(IOptions<HubConnectionInfo> options)
    {
        var connectionInfo = options.Value;
        _connection = new HubConnectionBuilder()
            .WithUrl(connectionInfo.Url)
            .WithAutomaticReconnect()
            .Build();

        _remoteProcedureName = connectionInfo.RemoteProcedureName;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        return _connection.StartAsync(cancellationToken);
    }

    public Task SendAsync(string message, CancellationToken cancellationToken = default)
    {
        return _connection.SendAsync(_remoteProcedureName, message, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _connection.DisposeAsync();
    }
}
