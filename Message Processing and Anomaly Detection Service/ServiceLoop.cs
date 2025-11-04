using System.Text.Json;
using Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances;
using Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEnginesManagers;
using Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEnginesManagers.EvaluationConstants;
using Message_Processing_and_Anomaly_Detection_Service.RabbitMQReceivers;
using Message_Processing_and_Anomaly_Detection_Service.SignalRSenders;

namespace Message_Processing_and_Anomaly_Detection_Service;

public class ServiceLoop
{
    private readonly IRabbitMQReceiver _queueReceiver;
    private readonly IPerformanceEvaluationEnginesManager _enginesManager;
    private readonly IDatabase _database;
    private readonly ISignalRSenderConnection _signalRConnection;
    private readonly bool _serviceIsRunning = true;
    public ServiceLoop(IRabbitMQReceiver queueReceiver,IPerformanceEvaluationEnginesManager enginesManager,
        IDatabase database,ISignalRSenderConnection signalRConnection)
    {
        _queueReceiver = queueReceiver;
        _enginesManager = enginesManager;
        _database = database;
        _signalRConnection = signalRConnection;
    }

    public async Task StartService()
    {
        await _signalRConnection.StartAsync();
        while (_serviceIsRunning)
        {
            var payload = await _queueReceiver.ReceivePayload();
            var data = JsonSerializer.Deserialize<ServerStatistics>(payload)!;
            
            var prevReading = await _database.GetLastAsync(data.ServerIdentifier);
            await _database.SaveStatisticsAsync(data);
            
            var alerts = _enginesManager.ExecuteEnginesRules(data,prevReading);

            var alertsSerialized = JsonSerializer.Serialize(alerts);
            await _signalRConnection.SendAsync(alertsSerialized);
        }
    }
}