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

        _ = await mediator.Send(new MemoryQuery(1));
        _ = await mediator.Send(new MemoryQuery(1));

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

        _ = await mediator.Send(new MemoryQuery(1));
        _ = await mediator.Send(new MemoryQuery(2));

        counter.Count.Should().Be(2);

        _ = await mediator.Send(new MemoryQuery(1));
        _ = await mediator.Send(new MemoryQuery(2));

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

        _ = await mediator.Send(new NoMemoryQuery(1));
        _ = await mediator.Send(new NoMemoryQuery(2));

        _ = await mediator.Send(new NoMemoryQuery(1));
        _ = await mediator.Send(new NoMemoryQuery(2));

        counter.Count.Should().Be(4);
    }
}

public class Counter
{
    public int Count { get; set; }
}

[MemoryCache]
public record MemoryQuery(int Parameter) : IRequest<int>;

public class MemoryQueryHandler : IRequestHandler<MemoryQuery, int>
{
    private readonly Counter _counter;

    public MemoryQueryHandler(Counter counter)
    {
        _counter = counter;
    }

    public Task<int> Handle(MemoryQuery request, CancellationToken cancellationToken)
    {
        _counter.Count++;
        return Task.FromResult(request.Parameter);
    }
}

public record NoMemoryQuery(int Parameter) : IRequest<int>;

public class NoMemoryQueryHandler : IRequestHandler<NoMemoryQuery, int>
{
    private readonly Counter _counter;

    public NoMemoryQueryHandler(Counter counter)
    {
        _counter = counter;
    }

    public Task<int> Handle(NoMemoryQuery request, CancellationToken cancellationToken)
    {
        _counter.Count++;
        return Task.FromResult(request.Parameter);
    }
}