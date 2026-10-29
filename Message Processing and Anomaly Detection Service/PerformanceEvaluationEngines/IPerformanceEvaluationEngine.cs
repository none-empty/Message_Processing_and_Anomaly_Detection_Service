using Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEnginesManagers.EvaluationConstants;

namespace Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEngines;

public interface IPerformanceEvaluationEngine
{
    public List<Alert> EvaluatePerformance(ServerStatistics currentStat,ServerStatistics prevStat,
        IEvaluationConstantsGetter evaluationConstants);
}