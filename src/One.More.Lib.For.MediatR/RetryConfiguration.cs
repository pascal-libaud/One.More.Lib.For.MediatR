namespace One.More.Lib.For.MediatR;

public partial class MediatRExtensionConfiguration
{
    public bool RetrySupport { get; set; } = false;

    public int RetryCount { get; set; } = 3;

    public int RetryDelay { get; set; } = 500;

    public MediatRExtensionConfiguration AddRetrySupport(int? retryCount = null, int? retryDelay = null)
    {
        RetrySupport = true;
        RetryCount = retryCount ?? RetryCount;
        RetryDelay = retryDelay ?? RetryDelay;
        return this;
    }
}