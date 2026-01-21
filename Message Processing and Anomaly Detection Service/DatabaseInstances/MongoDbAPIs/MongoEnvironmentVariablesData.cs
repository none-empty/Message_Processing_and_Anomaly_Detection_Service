namespace Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances.MongoDbAPIs;

public class MongoEnvironmentVariablesData : IMongoConnectionData
{
    public string ConnectionString { get; init; } = Environment.GetEnvironmentVariable("ConnectionString")!;
    public string DataBaseName { get; init; } = Environment.GetEnvironmentVariable("DataBaseName")!;
    public string CollectionName { get; set; } = Environment.GetEnvironmentVariable("CollectionName")!;
}