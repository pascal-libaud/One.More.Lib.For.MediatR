using Microsoft.Extensions.Logging;

namespace One.More.Lib.For.MediatR.Test;

public class RetryPipelineBehaviorTest
{
    private static string GetLog<T>()
    {
        return $"Failed to execute handler for {typeof(T).Name}, retrying after";
    }

    [Fact]
    public async Task RetryQueryHandler_should_work_as_expected()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .BuildServiceProvider()
            .GetMediator();

        var query = new RetryRequest { CountExceptions = 2 };

        var func = () => mediator.Send(query);

        await func.Should().ThrowAsync<RetryException>();
        await func.Should().ThrowAsync<RetryException>();
        await func.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Retry_pipeline_behavior_should_handle_exceptions_and_return_value()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureRetry()
            .AddSingleton(typeof(ILogger<>), typeof(SpyLogger<>))
            .BuildServiceProvider()
            .GetRequiredService<ILogger<RetryPipelineBehavior<RetryRequest, int>>>(out var loggerOfT)
            .GetMediator();

        var func = () => mediator.Send(new RetryRequest { CountExceptions = 2 });
        await func.Should().NotThrowAsync();

        loggerOfT.Should().NotBeNull();
        loggerOfT.Should().BeAssignableTo<SpyLogger>()
            .Which.Logs.Should().HaveCount(2)
            .And.Subject.Should().AllSatisfy(x => x.Should().StartWith(GetLog<RetryRequest>()));
    }

    [Fact]
    public async Task Retry_pipeline_behavior_should_not_handle_exceptions_when_not_active()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureRetry(active: false)
            .AddSingleton(typeof(ILogger<>), typeof(SpyLogger<>))
            .BuildServiceProvider()
            .GetRequiredService<ILogger<RetryPipelineBehavior<RetryRequest, int>>>(out var loggerOfT)
            .GetMediator();

        var func = () => mediator.Send(new RetryRequest { CountExceptions = 1 });
        await func.Should().ThrowAsync<RetryException>();

        loggerOfT.Should().NotBeNull();
        loggerOfT.Should().BeAssignableTo<SpyLogger>()
            .Which.Logs.Should().HaveCount(0);
    }

    [Fact]
    public async Task Retry_pipeline_behavior_should_handle_exceptions_and_return_value_when_on_limit_of_exceptions()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureRetry()
            .AddSingleton(typeof(ILogger<>), typeof(SpyLogger<>))
            .BuildServiceProvider()
            .GetRequiredService<ILogger<RetryPipelineBehavior<RetryRequest, int>>>(out var loggerOfT)
            .GetMediator();

        var func = () => mediator.Send(new RetryRequest { CountExceptions = 3 });
        await func.Should().NotThrowAsync();

        loggerOfT.Should().NotBeNull();
        loggerOfT.Should().BeAssignableTo<SpyLogger>()
            .Which.Logs.Should().HaveCount(3)
            .And.Subject.Should().AllSatisfy(x => x.Should().StartWith(GetLog<RetryRequest>()));
    }

    [Fact]
    public async Task Retry_pipeline_behavior_should_return_exception_when_too_much_exceptions()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureRetry()
            .AddSingleton(typeof(ILogger<>), typeof(SpyLogger<>))
            .BuildServiceProvider()
            .GetRequiredService<ILogger<RetryPipelineBehavior<RetryRequest, int>>>(out var loggerOfT)
            .GetMediator();

        var func = () => mediator.Send(new RetryRequest { CountExceptions = 4 });
        await func.Should().ThrowAsync<RetryException>();

        loggerOfT.Should().NotBeNull();
        loggerOfT.Should().BeAssignableTo<SpyLogger>()
            .Which.Logs.Should().HaveCount(3)
            .And.Subject.Should().AllSatisfy(x => x.Should().StartWith(GetLog<RetryRequest>()));
    }

    [Fact]
    public async Task Retry_pipeline_behavior_should_work_with_requests_with_no_return_value()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureRetry()
            .AddSingleton(typeof(ILogger<>), typeof(SpyLogger<>))
            .BuildServiceProvider()
            .GetRequiredService<ILogger<RetryPipelineBehavior<RetryVoidRequest, Unit>>>(out var loggerOfT)
            .GetMediator();

        var func = () => mediator.Send(new RetryVoidRequest { CountExceptions = 2 });
        await func.Should().NotThrowAsync();

        loggerOfT.Should().NotBeNull();
        loggerOfT.Should().BeAssignableTo<SpyLogger>()
            .Which.Logs.Should().HaveCount(2)
            .And.Subject.Should().AllSatisfy(x => x.Should().StartWith(GetLog<RetryVoidRequest>()));
    }

    [Fact]
    public async Task Retry_pipeline_behavior_should_take_account_of_retry_policy_properties()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureRetry()
            .AddSingleton(typeof(ILogger<>), typeof(SpyLogger<>))
            .BuildServiceProvider()
            .GetRequiredService<ILogger<RetryPipelineBehavior<RetryWithOverrideRequest, int>>>(out var loggerOfT)
            .GetMediator();

        var func = () => mediator.Send(new RetryWithOverrideRequest { CountExceptions = 3 });
        await func.Should().ThrowAsync<RetryException>();

        loggerOfT.Should().NotBeNull();
        loggerOfT.Should().BeAssignableTo<SpyLogger>()
            .Which.Logs.Should().HaveCount(1)
            .And.Subject.Should().AllSatisfy(x => x.Should().StartWith(GetLog<RetryWithOverrideRequest>()));
    }
}

[RetryPolicy]
public class RetryRequest : IRequest<int>
{
    public required int CountExceptions { get; set; }
}

public class RetryRequestHandler : IRequestHandler<RetryRequest, int>
{
    public Task<int> Handle(RetryRequest request, CancellationToken cancellationToken)
    {
        if (request.CountExceptions-- > 0)
            throw new RetryException();

        return Task.FromResult(request.CountExceptions);
    }
}

[RetryPolicy]
public class RetryVoidRequest : IRequest
{
    public required int CountExceptions { get; set; }
}

public class RetryVoidRequestHandler : IRequestHandler<RetryVoidRequest>
{
    public Task Handle(RetryVoidRequest request, CancellationToken cancellationToken)
    {
        if (request.CountExceptions-- > 0)
            throw new RetryException();

        return Task.CompletedTask;
    }
}

[RetryPolicy(OverrideRetryCount = true, RetryCount = 1)]
public class RetryWithOverrideRequest : IRequest<int>
{
    public required int CountExceptions { get; set; }
}

public class RetryWithOverrideRequestHandler : IRequestHandler<RetryWithOverrideRequest, int>
{
    public Task<int> Handle(RetryWithOverrideRequest request, CancellationToken cancellationToken)
    {
        if (request.CountExceptions-- > 0)
            throw new RetryException();

        return Task.FromResult(request.CountExceptions);
    }
}

public class RetryException : Exception;