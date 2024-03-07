using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace One.More.Lib.For.MediatR;

internal class PerformanceLoggerConfiguration
{
    internal int TriggerThreshold { get; set; }
}

internal class PerformanceLoggerPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<PerformanceLoggerPipelineBehavior<TRequest, TResponse>> _logger;
    private readonly PerformanceLoggerConfiguration _configuration;

    public PerformanceLoggerPipelineBehavior(ILogger<PerformanceLoggerPipelineBehavior<TRequest, TResponse>> logger, PerformanceLoggerConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            return await next();
        }
        finally
        {
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > _configuration.TriggerThreshold)
                _logger.LogInformation($"Handled {typeof(TRequest).Name} in {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}