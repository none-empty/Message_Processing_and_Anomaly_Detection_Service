namespace Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances.MongoDbAPIs;

public interface IMongoConnectionData
{
    public string ConnectionString { get; init; }
    public string DataBaseName { get; init; }
    public string CollectionName { get; set; }
}