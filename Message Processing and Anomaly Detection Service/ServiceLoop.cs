using System.Text.Json;
using Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances;
using Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEnginesManagers;
using Message_Processing_and_Anomaly_Detection_Service.RabbitMQReceivers;
using Message_Processing_and_Anomaly_Detection_Service.SignalRSenders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Message_Processing_and_Anomaly_Detection_Service;

public class ServiceLoop : BackgroundService
{
    private readonly IRabbitMQReceiver _queueReceiver;
    private readonly IPerformanceEvaluationEnginesManager _enginesManager;
    private readonly IDatabase _database;
    private readonly ISignalRSenderConnection _signalRConnection;
    private readonly ILogger<ServiceLoop> _logger;

    public ServiceLoop(IRabbitMQReceiver queueReceiver, IPerformanceEvaluationEnginesManager enginesManager,
        IDatabase database, ISignalRSenderConnection signalRConnection, ILogger<ServiceLoop> logger)
    {
        _queueReceiver = queueReceiver;
        _enginesManager = enginesManager;
        _database = database;
        _signalRConnection = signalRConnection;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await StartWithRetryAsync(_signalRConnection.StartAsync, "SignalR", stoppingToken);
        await StartWithRetryAsync(_queueReceiver.StartAsync, "RabbitMQ", stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var payload = await _queueReceiver.ReceivePayloadAsync(stoppingToken);
            try
            {
                var data = JsonSerializer.Deserialize<ServerStatistics>(payload)
                    ?? throw new JsonException("Received null server statistics.");

                var prevReading = await _database.GetLastAsync(data.ServerIdentifier);
                await _database.SaveStatisticsAsync(data);

                var alerts = _enginesManager.ExecuteEnginesRules(data, prevReading);
                var alertsSerialized = JsonSerializer.Serialize(alerts);
                await _signalRConnection.SendAsync(alertsSerialized, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Could not process received server statistics.");
            }
        }
    }

    private async Task StartWithRetryAsync(Func<CancellationToken, Task> start, string serviceName,
        CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                await start(stoppingToken);
                return;
            }
            catch (Exception exception) when (!stoppingToken.IsCancellationRequested &&
                                              (exception is BrokerUnreachableException or IOException or
                                                   HttpRequestException or TimeoutException ||
                                               exception is OperationInterruptedException
                                               {
                                                   ShutdownReason: { Initiator: ShutdownInitiator.Library } or
                                                       { ReplyCode: Constants.ConnectionForced }
                                               }))
            {
                _logger.LogWarning(exception, "Could not start {ServiceName}. Retrying in five seconds.", serviceName);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
