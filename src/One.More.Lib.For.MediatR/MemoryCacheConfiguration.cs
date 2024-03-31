using System;
using Microsoft.Extensions.Caching.Memory;

namespace One.More.Lib.For.MediatR;

public partial class MediatRExtensionConfiguration
{
    public bool MemoryCacheSupport { get; set; } = false;

    internal DateTimeOffset? AbsoluteExpiration { get; private set; }

    internal TimeSpan? AbsoluteExpirationRelativeToNow { get; private set; }

    internal TimeSpan? SlidingExpiration { get; private set; }

    internal CacheItemPriority Priority { get; private set; }

    public MediatRExtensionConfiguration AddMemoryCacheSupport(DateTimeOffset? absoluteExpiration = null, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null, CacheItemPriority priority = CacheItemPriority.Normal)
    {
        MemoryCacheSupport = true;
        AbsoluteExpiration = absoluteExpiration;
        AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        SlidingExpiration = slidingExpiration;
        Priority = priority;

        return this;
    }
}