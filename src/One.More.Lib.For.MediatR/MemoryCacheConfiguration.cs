using System;
using Microsoft.Extensions.Caching.Memory;

namespace One.More.Lib.For.MediatR;

public partial class MediatRExtensionConfiguration
{
    public bool MemoryCacheSupport { get; set; } = false;

    internal MemoryCacheConfiguration MemoryCacheConfiguration { get; set; } = new();

    public MediatRExtensionConfiguration AddMemoryCacheSupport(
        DateTimeOffset? absoluteExpiration = null,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null,
        CacheItemPriority priority = CacheItemPriority.Normal)
    {
        MemoryCacheSupport = true;

        MemoryCacheConfiguration.MainConfiguration.AbsoluteExpiration = absoluteExpiration;
        MemoryCacheConfiguration.MainConfiguration.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        MemoryCacheConfiguration.MainConfiguration.SlidingExpiration = slidingExpiration;
        MemoryCacheConfiguration.MainConfiguration.Priority = priority;

        return this;
    }

    public MediatRExtensionConfiguration AddMemoryCacheSupportOverrideFor<T>(
        DateTimeOffset? absoluteExpiration = null,
        TimeSpan? absoluteExpirationRelativeToNow = null,
        TimeSpan? slidingExpiration = null,
        CacheItemPriority priority = CacheItemPriority.Normal)
    {
        var conf = new MemoryCacheItemConfiguration
        {
            AbsoluteExpiration = absoluteExpiration,
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
            SlidingExpiration = slidingExpiration,
            Priority = priority,
        };

        if (!MemoryCacheConfiguration.OverrideConfigurations.TryAdd(typeof(T), conf))
            MemoryCacheConfiguration.OverrideConfigurations[typeof(T)] = conf;

        return this;
    }
}