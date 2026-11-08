using Message_Processing_and_Anomaly_Detection_Service.Options;

namespace Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEngines;

public interface IPerformanceEvaluationEngine
{
    public List<Alert> EvaluatePerformance(ServerStatistics currentStat,ServerStatistics prevStat,
        EvaluationParameters evaluationParameters);
}
