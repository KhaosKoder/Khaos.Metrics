namespace Khaos.Metrics;

public interface IOperationEventSink
{
    void OnOperationCompleted(OperationCompletedContext context);
}
