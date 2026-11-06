namespace Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances.MongoDbAPIs;

public class MongoConnectionData
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DataBaseName { get; set; } = string.Empty;
    public string CollectionName { get; set; } = string.Empty;
}
