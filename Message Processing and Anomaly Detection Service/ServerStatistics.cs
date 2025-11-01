namespace Message_Processing_and_Anomaly_Detection_Service;

public record ServerStatistics(double MemoryUsage, double AvailableMemory, double CpuUsage, 
    DateTime Timestamp);