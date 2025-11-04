using MongoDB.Driver;

namespace Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances.MongoDbAPIs;

public class MongoDbInstance : IDatabase
{
    private readonly IMongoCollection<ServerStatisticsDocument> _statsCollection;
     
    public MongoDbInstance(IMongoConnectionData mongoConnectionData,IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase(mongoConnectionData.DataBaseName);
        _statsCollection = database.GetCollection<ServerStatisticsDocument>(mongoConnectionData.CollectionName);
    }
    public async Task SaveStatisticsAsync(ServerStatistics stats)
    {
        var statsDocument = new ServerStatisticsDocument{
            ServerIdentifier = stats.ServerIdentifier,
            MemoryUsage = stats.MemoryUsage,
            AvailableMemory = stats.AvailableMemory,
            CpuUsage = stats.CpuUsage,
            Timestamp = stats.Timestamp
        };
    
        
        await _statsCollection.InsertOneAsync(statsDocument);
    }

    public async Task<ServerStatistics> GetLastAsync(string serverIdentifier)
    {  
        var lastEntry = await _statsCollection
            .Find(doc => doc.ServerIdentifier.Equals(serverIdentifier))
            .SortByDescending(doc => doc.Timestamp)
            .FirstOrDefaultAsync();


        return new ServerStatistics(
            lastEntry.MemoryUsage,
            lastEntry.AvailableMemory,
            lastEntry.CpuUsage,
            lastEntry.Timestamp,
            lastEntry.ServerIdentifier
            );
    }
}