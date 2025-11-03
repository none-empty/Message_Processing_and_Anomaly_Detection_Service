namespace Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEnginesManagers.EvaluationConstants;

public class EvaluationConstantsGetterFromEnvironmentVariables : IEvaluationConstantsGetter
{
    public double MemoryUsageAnomalyThresholdPercentage { get; init; }
    public double MemoryUsageThresholdPercentage { get; init; }
    public double CpuUsageAnomalyThresholdPercentage { get; init; }
    public double CpuUsageThresholdPercentage { get; init; }

    public EvaluationConstantsGetterFromEnvironmentVariables()
    {
        {
            MemoryUsageAnomalyThresholdPercentage = double.TryParse(Environment
                    .GetEnvironmentVariable(
                        nameof(MemoryUsageAnomalyThresholdPercentage)), out var value
            )
                ? value
                : 0;
        }
        
        {
            MemoryUsageThresholdPercentage = double.TryParse(Environment
                    .GetEnvironmentVariable(
                        nameof(MemoryUsageThresholdPercentage)), out var value
            )
                ? value
                : 0;
        }
        
        {
            CpuUsageAnomalyThresholdPercentage = double.TryParse(Environment
                    .GetEnvironmentVariable(
                        nameof(CpuUsageAnomalyThresholdPercentage)), out var value
            )
                ? value
                : 0;
        }
        
        {
            CpuUsageThresholdPercentage = double.TryParse(Environment
                    .GetEnvironmentVariable(
                        nameof(CpuUsageThresholdPercentage)), out var value
            )
                ? value
                : 0;
        }
    }
}