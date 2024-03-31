using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace One.More.Lib.For.MediatR;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class MemoryCacheAttribute : Attribute
{
    public bool IsActive { get; set; } = true;

    public DateTimeOffset? AbsoluteExpiration { get; set; }

    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

    public TimeSpan? SlidingExpiration { get; set; }
    
    public CacheItemPriority? Priority { get; set; }
}

internal class MemoryCacheConfiguration
{
    internal DateTimeOffset? AbsoluteExpiration { get; set; }

    internal TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

    internal TimeSpan? SlidingExpiration { get; set; }

    internal CacheItemPriority Priority { get; set; }
}

internal class MemoryCachePipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheConfiguration _configuration;

    public MemoryCachePipelineBehavior(IMemoryCache memoryCache, MemoryCacheConfiguration configuration)
    {
        _memoryCache = memoryCache;
        _configuration = configuration;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var memoryPolicy = typeof(TRequest).GetCustomAttribute<MemoryCacheAttribute>();

        if (memoryPolicy == null || !memoryPolicy.IsActive)
            return await next();

        if (!_memoryCache.TryGetValue(request, out TResponse result))
        {
            result = await next();
            _memoryCache.Set(request, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = memoryPolicy.AbsoluteExpiration ?? _configuration.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = memoryPolicy.AbsoluteExpirationRelativeToNow ?? _configuration.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = memoryPolicy.SlidingExpiration ?? _configuration.SlidingExpiration,
                Priority = memoryPolicy.Priority ?? _configuration.Priority,
            });
        }

        return result;
    }
}