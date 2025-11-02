using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances.MongoDbAPIs;

public class ServerStatisticsDocument
{
    [BsonId] [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }= null!;
    public double MemoryUsage { get; set; }
    public double AvailableMemory { get; set; }
    public double CpuUsage { get; set; }
    public DateTime Timestamp { get; set; }

    public ServerStatisticsDocument()
    {
        
    }

    public ServerStatisticsDocument(double memoryUsage, double availableMemory, double cpuUsage, DateTime timestamp)
    {
        MemoryUsage = memoryUsage;
        AvailableMemory = availableMemory;
        CpuUsage = cpuUsage;
        Timestamp = timestamp;
    }
}