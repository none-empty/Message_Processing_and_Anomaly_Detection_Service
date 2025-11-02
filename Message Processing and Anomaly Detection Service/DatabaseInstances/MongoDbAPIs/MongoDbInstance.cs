using MongoDB.Driver;

namespace Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances.MongoDbAPIs;

public class MongoDbInstance : IDatabase
{
    private readonly IMongoCollection<ServerStatisticsDocument> _statsCollection;
    
    public MongoDbInstance(IMongoConnectionData mongoConnectionData)
    {
        var client = new MongoClient(mongoConnectionData.ConnectionString);
        var database = client.GetDatabase(mongoConnectionData.DataBaseName);
        _statsCollection = database.GetCollection<ServerStatisticsDocument>(mongoConnectionData.CollectionName);
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

    public async Task<ServerStatistics> GetLast()
    {
        var lastEntry = await _statsCollection
            .Find(_ => true) 
            .SortByDescending(doc => doc.Id)  
            .FirstOrDefaultAsync();


        return new ServerStatistics(
            lastEntry.MemoryUsage,
            lastEntry.AvailableMemory,
            lastEntry.CpuUsage,
            lastEntry.Timestamp
            );
    }
}