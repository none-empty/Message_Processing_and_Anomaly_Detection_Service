using Microsoft.AspNetCore.SignalR;

namespace DataHub;

public class AlertsHub : Hub
{
    public async Task SendAlert(string message)
    {
        await Clients.All.SendAsync("ReceiveAlert", message);
    }
}