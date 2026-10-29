using Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEnginesManagers.EvaluationConstants;

namespace Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEngines;

public class MemoryPerformanceEvaluationEngine : IPerformanceEvaluationEngine
{
 
    public List<Alert> EvaluatePerformance(ServerStatistics currentStat, ServerStatistics prevStat,
        IEvaluationConstantsGetter evaluationConstants)
    {
        var res = new List<Alert>();
        
        var (issueDetected, alert) = CheckForMemoryUsageAnomaly(currentStat, prevStat
            , evaluationConstants.MemoryUsageAnomalyThresholdPercentage);
        
        if(issueDetected)res.Add(alert);

        (issueDetected, alert) = CheckForMemoryHighUsage(currentStat, 
            evaluationConstants.MemoryUsageThresholdPercentage);
        
        if(issueDetected)res.Add(alert);
        
        return res;
    }
    
    private (bool,Alert)CheckForMemoryUsageAnomaly(ServerStatistics currentReading,ServerStatistics prevReading
        ,double memoryUsageAnomalyThresholdPercentage)
    {
        double currentMemoryUsage = currentReading.MemoryUsage
            , prevMemoryUsage = prevReading.MemoryUsage;
        
        return (currentMemoryUsage > (prevMemoryUsage * (1 + memoryUsageAnomalyThresholdPercentage)))
            ? (true, new Alert
                ($"Memory Usage Anomaly Detected in {currentReading.ServerIdentifier} at {currentReading.Timestamp}"))
            
            : (false, new Alert(""));
    }

    private (bool,Alert) CheckForMemoryHighUsage(ServerStatistics currentReading,double memoryUsageThresholdPercentage)
    {
        double currentMemoryUsage = currentReading.MemoryUsage,
            currentAvailableMemory = currentReading.AvailableMemory;
        
        return ((currentMemoryUsage / (currentMemoryUsage + currentAvailableMemory)) > memoryUsageThresholdPercentage)
            ? (true, new Alert
                ($"Memory High Usage Detected in {currentReading.ServerIdentifier} at {currentReading.Timestamp}"))
            : (false, new Alert(""));
    }

 
}