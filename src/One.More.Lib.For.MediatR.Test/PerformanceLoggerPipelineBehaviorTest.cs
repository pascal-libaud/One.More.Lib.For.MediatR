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
            .GetRequiredService<ILogger<PerformanceLoggerPipelineBehavior<DelayRequest, bool>>>(out var loggerOfT)
            .GetMediator();

        var delay = await mediator.Send(new DelayRequest(300));
        delay.Should().Be(true);

        loggerOfT.Should().NotBeNull();
        loggerOfT.Should().BeAssignableTo<SpyLogger>()
            .Which.Logs.Should().HaveCount(1)
            .And.Subject.First().Should().StartWith($"Handled {nameof(DelayRequest)} in");
    }

    [Fact]
    public async Task Performance_logger_pipeline_behavior_should_not_log_when_not_active()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigurePerformanceLogger(active: false)
            .AddSingleton(typeof(ILogger<>), typeof(SpyLogger<>))
            .BuildServiceProvider()
            .GetRequiredService<ILogger<PerformanceLoggerPipelineBehavior<DelayRequest, bool>>>(out var loggerOfT)
            .GetMediator();

        var delay = await mediator.Send(new DelayRequest(300));
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
            .GetRequiredService<ILogger<PerformanceLoggerPipelineBehavior<DelayRequest, bool>>>(out var loggerOfT)
            .GetMediator();

        var delay = await mediator.Send(new DelayRequest(0));
        delay.Should().Be(true);

        loggerOfT.Should().NotBeNull();
        loggerOfT.Should().BeAssignableTo<SpyLogger>().Which.Logs.Should().HaveCount(0);
    }

    [Fact]
    public async Task Performance_logger_pipeline_behavior_should_work_with_requests_with_no_return_value()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigurePerformanceLogger()
            .AddSingleton(typeof(ILogger<>), typeof(SpyLogger<>))
            .BuildServiceProvider()
            .GetRequiredService<ILogger<PerformanceLoggerPipelineBehavior<DelayVoidRequest, Unit>>>(out var loggerOfT)
            .GetMediator();

        await mediator.Send(new DelayVoidRequest(300));

        loggerOfT.Should().NotBeNull();
        loggerOfT.Should().BeAssignableTo<SpyLogger>()
            .Which.Logs.Should().HaveCount(1)
            .And.Subject.First().Should().StartWith($"Handled {nameof(DelayVoidRequest)} in ");
    }
}

public record DelayRequest(int DelayInMilliseconds) : IRequest<bool>;

public class DelayRequestHandler : IRequestHandler<DelayRequest, bool>
{
    public async Task<bool> Handle(DelayRequest request, CancellationToken cancellationToken)
    {
        await Task.Delay(request.DelayInMilliseconds, cancellationToken);
        return true;
    }
}

public record DelayVoidRequest(int DelayInMilliseconds) : IRequest;

public class DelayVoidRequestHandler : IRequestHandler<DelayVoidRequest>
{
    public async Task Handle(DelayVoidRequest request, CancellationToken cancellationToken)
    {
        await Task.Delay(request.DelayInMilliseconds, cancellationToken);
    }
}