namespace One.More.Lib.For.MediatR;

public partial class MediatRExtensionConfiguration
{
    public bool PerformanceLoggerSupport { get; set; } = false;

    public int TriggerThreshold { get; set; } = 0;

    public MediatRExtensionConfiguration AddPerformanceLoggerSupport(int triggerThreshold = 0)
    {
        PerformanceLoggerSupport = true;
        TriggerThreshold = triggerThreshold;
        return this;
    }
}