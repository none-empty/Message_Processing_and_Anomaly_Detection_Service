using System.Reflection;
using Message_Processing_and_Anomaly_Detection_Service.Options;
using Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEngines;
using Microsoft.Extensions.Options;

namespace Message_Processing_and_Anomaly_Detection_Service.PerformanceEvaluationEnginesManagers;

public class PerformanceEvaluationEnginesManager : IPerformanceEvaluationEnginesManager
{
    private readonly List<IPerformanceEvaluationEngine> _engines;
    private readonly EvaluationParameters _evaluationParameters;
        
    public PerformanceEvaluationEnginesManager(IOptions<EvaluationParameters> options)
    {
        var types = GetEnginesTypes();
        _engines = CreateEngines(types);
        _evaluationParameters = options.Value;
    }

    private List<Type> GetEnginesTypes()
    {
        Type interfaceType = typeof(IPerformanceEvaluationEngine);
        Assembly assembly = Assembly.GetExecutingAssembly();
        
        var types = assembly
            .GetTypes()
            .Where(type => interfaceType.IsAssignableFrom(type) && !type.IsInterface)
            .ToList();

        return types;
    }

    private List<IPerformanceEvaluationEngine>CreateEngines(List<Type> types)
    {
        return types.Select(type => (IPerformanceEvaluationEngine)Activator.CreateInstance(type)!).ToList();  
    }
    
    public List<Alert> ExecuteEnginesRules(ServerStatistics currentStats,ServerStatistics prevStats)
    {
        var alerts = _engines
            .SelectMany(engine => engine.EvaluatePerformance(currentStats,prevStats,_evaluationParameters))
            .ToList();

        return alerts;
    }
}
