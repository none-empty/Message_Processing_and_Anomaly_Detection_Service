using System.Data;
using Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances;
using Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances.MongoDbAPIs;
using Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEnginesManagers;
using Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEnginesManagers.EvaluationConstants;
using Message_Processing_and_Anomaly_Detection_Service.RabbitMQReceivers;
using Message_Processing_and_Anomaly_Detection_Service.SignalRSenders;
using Message_Processing_and_Anomaly_Detection_Service.SignalRSenders.SignalRConnectionInfo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace Message_Processing_and_Anomaly_Detection_Service;

class Program
{
    static async Task Main(string[] args)
    {
        var connectionString = new MongoEnvironmentVariablesData().ConnectionString;
        var host = Host.CreateDefaultBuilder(args) .ConfigureServices((context, services) =>
        {
            services.AddSingleton<IRabbitMQReceiver, RabbitMQReceiver>();
            services.AddSingleton<IConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = Environment.GetEnvironmentVariable("RABBITMQ__HOSTNAME") ?? "localhost",
                    Port = int.TryParse(Environment.GetEnvironmentVariable("RABBITMQ__PORT"), out var port)
                        ? port
                        : 5672,
                    UserName = Environment.GetEnvironmentVariable("RABBITMQ__USERNAME") ?? "guest",
                    Password = Environment.GetEnvironmentVariable("RABBITMQ__PASSWORD") ?? "guest"
                };

                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });
            services.AddSingleton<IMongoConnectionData, MongoEnvironmentVariablesData>();
            services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));
            services.AddSingleton<IDatabase,MongoDbInstance>();
            services.AddSingleton<IEvaluationConstantsGetter, EvaluationConstantsGetterFromEnvironmentVariables>();
            services.AddSingleton<IPerformanceEvaluationEnginesManager,PerformanceEvaluationEnginesManager>();
            services.AddSingleton<IHubConnectionInfo, HubConnectionInfoFromEnvironmentVariables>();
            services.AddSingleton<ISignalRSenderConnection, SignalRSenderConnection>();
            services.AddSingleton<ServiceLoop>(); 
        }) .Build();

         
        var service = host.Services.GetRequiredService<ServiceLoop>();
        await service.StartService();
    }
}