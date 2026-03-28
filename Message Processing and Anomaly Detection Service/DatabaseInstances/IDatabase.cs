namespace Message_Processing_and_Anomaly_Detection_Service.DatabaseInstances;

public interface IDatabase
{
    public Task SaveStatisticsAsync(ServerStatistics stats);
    public Task<ServerStatistics> GetLastAsync(string serverIdentifier);
}