using Microsoft.Extensions.Logging;

namespace One.More.Lib.For.MediatR.Test;

public class PerformanceLoggerPipelineBehaviorTest
{
    [Fact]
    public async Task Performance_logger_pipeline_behavior_should_log_slow_queries()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigurePerformanceLogger()
            .AddSingleton(typeof(ILogger<>), typeof(SpyLogger<>))
            .BuildServiceProvider()
            .GetRequiredService<ILogger<PerformanceLoggerPipelineBehavior<DelayQuery, bool>>>(out var loggerOfT)
            .GetMediator();

        var delay = await mediator.Send(new DelayQuery(300));
        delay.Should().Be(true);

        loggerOfT.Should().NotBeNull();
        loggerOfT.Should().BeAssignableTo<SpyLogger>()
            .Which.Logs.Should().HaveCount(1)
            .And.Subject.First().Should().StartWith("Handled DelayQuery in");
    }

    [Fact]
    public async Task Performance_logger_pipeline_behavior_should_not_log_when_not_active()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigurePerformanceLogger(active: false)
            .AddSingleton(typeof(ILogger<>), typeof(SpyLogger<>))
            .BuildServiceProvider()
            .GetRequiredService<ILogger<PerformanceLoggerPipelineBehavior<DelayQuery, bool>>>(out var loggerOfT)
            .GetMediator();

        var delay = await mediator.Send(new DelayQuery(300));
        delay.Should().Be(true);

        loggerOfT.Should().NotBeNull();
        loggerOfT.Should().BeAssignableTo<SpyLogger>().Which.Logs.Should().HaveCount(0);
    }

    [Fact]
    public async Task Performance_logger_pipeline_behavior_should_not_log_fast_queries()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigurePerformanceLogger()
            .AddSingleton(typeof(ILogger<>), typeof(SpyLogger<>))
            .BuildServiceProvider()
            .GetRequiredService<ILogger<PerformanceLoggerPipelineBehavior<DelayQuery, bool>>>(out var loggerOfT)
            .GetMediator();

        var delay = await mediator.Send(new DelayQuery(0));
        delay.Should().Be(true);

        loggerOfT.Should().NotBeNull();
        loggerOfT.Should().BeAssignableTo<SpyLogger>().Which.Logs.Should().HaveCount(0);
    }
}

public record DelayQuery(int DelayInMilliseconds) : IRequest<bool>;

public class DelayQueryHandler : IRequestHandler<DelayQuery, bool>
{
    public async Task<bool> Handle(DelayQuery request, CancellationToken cancellationToken)
    {
        await Task.Delay(request.DelayInMilliseconds, cancellationToken);
        return true;
    }
}

public class SpyLogger : ILogger
{
    private readonly List<string> _logs = new();
    public IReadOnlyList<string> Logs => _logs;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logs.Add(formatter(state, null));
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}

public class SpyLogger<T> : SpyLogger, ILogger<T>;