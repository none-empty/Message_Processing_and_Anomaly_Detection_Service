namespace Message_Processing_and_Anomaly_Detection_Service.Options;

public class EvaluationParameters
{
    public double MemoryUsageAnomalyThresholdPercentage { get; set; }
    public double MemoryUsageThresholdPercentage { get; set; }
    public double CpuUsageAnomalyThresholdPercentage { get; set; }
    public double CpuUsageThresholdPercentage { get; set; }
}
