using Microsoft.Extensions.Caching.Memory;

namespace One.More.Lib.For.MediatR.Test;

public class MemoryCachePipelineBehaviorTest
{
    [Fact]
    public async Task Memory_cache_pipeline_behavior_should_not_use_memory_cache_when_not_active()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureMemoryCache(active: false)
            .AddSingleton<Counter>()
            .BuildServiceProvider()
            .GetRequiredService<Counter>(out var counter)
            .GetMediator();

        _ = await mediator.Send(new MemoryRequest(1));
        _ = await mediator.Send(new MemoryRequest(1));

        counter.Count.Should().Be(2);
    }

    [Fact]
    public async Task Memory_cache_pipeline_behavior_should_use_memory_cache_on_queries_with_memory_cache_attribute()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureMemoryCache()
            .AddSingleton<Counter>()
            .BuildServiceProvider()
            .GetRequiredService<Counter>(out var counter)
            .GetMediator();

        _ = await mediator.Send(new MemoryRequest(1));
        _ = await mediator.Send(new MemoryRequest(2));

        counter.Count.Should().Be(2);

        _ = await mediator.Send(new MemoryRequest(1));
        _ = await mediator.Send(new MemoryRequest(2));

        counter.Count.Should().Be(2);
    }

    [Fact]
    public async Task Memory_cache_pipeline_behavior_should_not_use_memory_cache_on_queries_without_memory_cache_attribute()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureMemoryCache()
            .AddSingleton<Counter>()
            .BuildServiceProvider()
            .GetRequiredService<Counter>(out var counter)
            .GetMediator();

        _ = await mediator.Send(new NoMemoryRequest(1));
        _ = await mediator.Send(new NoMemoryRequest(2));

        _ = await mediator.Send(new NoMemoryRequest(1));
        _ = await mediator.Send(new NoMemoryRequest(2));

        counter.Count.Should().Be(4);
    }

    [Fact]
    public async Task Memory_cache_pipeline_behavior_should_take_account_of_override_configuration()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureMemoryCacheDecorator()
            .ConfigureMemoryCacheAndOverrideFor<MemoryRequest>()
            .AddSingleton<Counter>()
            .BuildServiceProvider()
            .GetRequiredService<Counter>(out var counter)
            .GetRequiredService<IMemoryCache>(out var spyMemoryCache)
            .GetMediator();

        _ = await mediator.Send(new MemoryRequest(1));
        _ = await mediator.Send(new MemoryRequest(2));

        counter.Count.Should().Be(2);

        _ = await mediator.Send(new MemoryRequest(1));
        _ = await mediator.Send(new MemoryRequest(2));

        counter.Count.Should().Be(2);

        spyMemoryCache.Should().BeAssignableTo<SpyMemoryCache>()
            .Which.CacheEntries.Should().HaveCount(2)
            .And.AllSatisfy(x => x.Priority.Should().Be(CacheItemPriority.High));
    }
}

public class Counter
{
    public int Count { get; set; }
}

[MemoryCache]
public record MemoryRequest(int Parameter) : IRequest<int>;

public class MemoryRequestHandler : IRequestHandler<MemoryRequest, int>
{
    private readonly Counter _counter;

    public MemoryRequestHandler(Counter counter)
    {
        _counter = counter;
    }

    public Task<int> Handle(MemoryRequest request, CancellationToken cancellationToken)
    {
        _counter.Count++;
        return Task.FromResult(request.Parameter);
    }
}

public record NoMemoryRequest(int Parameter) : IRequest<int>;

public class NoMemoryRequestHandler : IRequestHandler<NoMemoryRequest, int>
{
    private readonly Counter _counter;

    public NoMemoryRequestHandler(Counter counter)
    {
        _counter = counter;
    }

    public Task<int> Handle(NoMemoryRequest request, CancellationToken cancellationToken)
    {
        _counter.Count++;
        return Task.FromResult(request.Parameter);
    }
}