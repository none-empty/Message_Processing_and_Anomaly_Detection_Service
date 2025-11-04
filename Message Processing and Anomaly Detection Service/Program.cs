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

namespace Message_Processing_and_Anomaly_Detection_Service;

class Program
{
    static async Task Main(string[] args)
    {
        var connectionString = new MongoEnvironmentVariablesData().ConnectionString;
        var host = Host.CreateDefaultBuilder(args) .ConfigureServices((context, services) =>
        {
            services.AddSingleton<IRabbitMQReceiver, RabbitMQReceiver>();
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