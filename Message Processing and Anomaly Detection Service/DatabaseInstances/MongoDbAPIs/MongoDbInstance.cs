using MongoDB.Driver;

namespace Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances.MongoDbAPIs;

public class MongoDbInstance : IDatabase
{
    private readonly IMongoCollection<ServerStatisticsDocument> _statsCollection;
    
    public MongoDbInstance(IConnectionData connectionData)
    {
        var client = new MongoClient(connectionData.ConnectionString);
        var database = client.GetDatabase(connectionData.DataBaseName);
        _statsCollection = database.GetCollection<ServerStatisticsDocument>(connectionData.CollectionName);
    }
    public async Task SaveStatisticsAsync(ServerStatistics stats)
    {
        var statsDocument = new ServerStatisticsDocument(
            stats.MemoryUsage,
            stats.AvailableMemory,
            stats.CpuUsage,
            stats.Timestamp
        );
    
        
        await _statsCollection.InsertOneAsync(statsDocument);
    }
}