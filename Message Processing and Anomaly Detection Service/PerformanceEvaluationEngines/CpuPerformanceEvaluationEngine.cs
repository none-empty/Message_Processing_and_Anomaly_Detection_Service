using Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEnginesManagers.EvaluationConstants;

namespace Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEngines;

public class CpuPerformanceEvaluationEngine : IPerformanceEvaluationEngine
{
    public List<Alert> EvaluatePerformance(ServerStatistics currentStat, ServerStatistics prevStat,
        IEvaluationConstantsGetter evaluationConstants)
    {
        var res = new List<Alert>();
        
        var (issueDetected, alert) = CheckForCpuUsageAnomaly(currentStat, prevStat
            , evaluationConstants.CpuUsageAnomalyThresholdPercentage);
        
        if(issueDetected)res.Add(alert);

        (issueDetected, alert) = CheckForCpuHighUsage(currentStat, evaluationConstants.CpuUsageThresholdPercentage);
        
        if(issueDetected)res.Add(alert);
        
        return res;
    }
    
    private (bool,Alert)CheckForCpuUsageAnomaly(ServerStatistics currentReading,ServerStatistics prevReading
        ,double cpuUsageAnomalyThresholdPercentage)
    {
        double currentCpuUsage = currentReading.CpuUsage
            , prevCpuUsage = prevReading.CpuUsage;
        
        return (currentCpuUsage > (prevCpuUsage * (1 + cpuUsageAnomalyThresholdPercentage)))
            ? (true, new Alert
                ($"CPU Usage Anomaly Detected in {currentReading.ServerIdentifier} at {currentReading.Timestamp}"))
            
            : (false, new Alert(""));
    }

    private (bool,Alert) CheckForCpuHighUsage(ServerStatistics currentReading,double cpuUsageThresholdPercentage)
    {
        return (currentReading.CpuUsage > cpuUsageThresholdPercentage)
            ? (true, new Alert
                ($"CPU High Usage Detected in {currentReading.ServerIdentifier} at {currentReading.Timestamp}"))
            : (false, new Alert(""));
    }
}