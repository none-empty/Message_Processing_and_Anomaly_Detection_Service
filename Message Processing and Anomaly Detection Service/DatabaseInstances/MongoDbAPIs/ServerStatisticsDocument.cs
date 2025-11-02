using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances.MongoDbAPIs;

public class ServerStatisticsDocument
{
    [BsonId] [BsonRepresentation(BsonType.ObjectId)]
    private string Id { get; set; }= null!;
    public double MemoryUsage { get; set; }
    private double AvailableMemory { get; set; }
    private double CpuUsage { get; set; }
    private DateTime Timestamp { get; set; }

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