namespace Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEnginesManagers.EvaluationConstants;

public interface IEvaluationConstantsGetter
{
    public double MemoryUsageAnomalyThresholdPercentage { get; init; }
    public double MemoryUsageThresholdPercentage { get; init; }
    public double CpuUsageAnomalyThresholdPercentage { get; init; }
    public double CpuUsageThresholdPercentage { get; init; }
}