using Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances;
using Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances.MongoDbAPIs;
using Message_Processing_and_Anomaly_Detection_Service.Options;
using Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEnginesManagers;
using Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEnginesManagers.EvaluationConstants;
using Message_Processing_and_Anomaly_Detection_Service.RabbitMQReceivers;
using Message_Processing_and_Anomaly_Detection_Service.SignalRSenders;
using Message_Processing_and_Anomaly_Detection_Service.SignalRSenders.SignalRConnectionInfo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace Message_Processing_and_Anomaly_Detection_Service;

class Program
{
    static async Task Main(string[] args)
    {
        var connectionString = new MongoEnvironmentVariablesData().ConnectionString;
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddOptions<RabbitMQOptions>()
                    .Bind(context.Configuration.GetSection("RabbitMQ"))
                    .Validate(options => options.BufferCapacity > 0, "RabbitMQ buffer capacity must be positive.")
                    .Validate(options => options.PrefetchCount > 0, "RabbitMQ prefetch count must be positive.")
                    .ValidateOnStart();
                services.AddSingleton<IConnectionFactory>(sp =>
                {
                    var options = sp.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
                    return new ConnectionFactory
                    {
                        HostName = options.HostName,
                        Port = options.Port,
                        UserName = options.UserName,
                        Password = options.Password,
                        AutomaticRecoveryEnabled = true,
                        TopologyRecoveryEnabled = true
                    };
                });
                services.AddSingleton<IRabbitMQReceiver, RabbitMQReceiver>();
                services.AddSingleton<IMongoConnectionData, MongoEnvironmentVariablesData>();
                services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));
                services.AddSingleton<IDatabase, MongoDbInstance>();
                services.AddSingleton<IEvaluationConstantsGetter, EvaluationConstantsGetterFromEnvironmentVariables>();
                services.AddSingleton<IPerformanceEvaluationEnginesManager, PerformanceEvaluationEnginesManager>();
                services.AddSingleton<IHubConnectionInfo, HubConnectionInfoFromEnvironmentVariables>();
                services.AddSingleton<ISignalRSenderConnection, SignalRSenderConnection>();
                services.AddHostedService<ServiceLoop>();
            })
            .Build();

        await host.RunAsync();
    }
}
