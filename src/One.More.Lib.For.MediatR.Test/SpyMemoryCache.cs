using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace One.More.Lib.For.MediatR.Test;

public class SpyMemoryCache : IMemoryCache
{
    private readonly MemoryCache _memoryCacheImplementation;

    public SpyMemoryCache(MemoryCache memoryCacheImplementation)
    {
        _memoryCacheImplementation = memoryCacheImplementation;
    }

    internal List<SpyCacheEntry> CacheEntries { get; } = new();

    public void Dispose()
    {
        _memoryCacheImplementation.Dispose();
    }

    public bool TryGetValue(object key, out object? value)
    {
        return _memoryCacheImplementation.TryGetValue(key, out value);
    }

    public ICacheEntry CreateEntry(object key)
    {
        var cacheEntry = new SpyCacheEntry(_memoryCacheImplementation.CreateEntry(key));
        CacheEntries.Add(cacheEntry);
        return cacheEntry;
    }

    public void Remove(object key)
    {
        _memoryCacheImplementation.Remove(key);
    }
}

public class SpyCacheEntry : ICacheEntry
{
    private readonly ICacheEntry _cacheEntryImplementation;

    public SpyCacheEntry(ICacheEntry cacheEntryImplementation)
    {
        _cacheEntryImplementation = cacheEntryImplementation;
    }

    public void Dispose()
    {
        _cacheEntryImplementation.Dispose();
    }

    public object Key => _cacheEntryImplementation.Key;

    public object? Value
    {
        get => _cacheEntryImplementation.Value;
        set => _cacheEntryImplementation.Value = value;
    }

    public DateTimeOffset? AbsoluteExpiration
    {
        get => _cacheEntryImplementation.AbsoluteExpiration;
        set => _cacheEntryImplementation.AbsoluteExpiration = value;
    }

    public TimeSpan? AbsoluteExpirationRelativeToNow
    {
        get => _cacheEntryImplementation.AbsoluteExpirationRelativeToNow;
        set => _cacheEntryImplementation.AbsoluteExpirationRelativeToNow = value;
    }

    public TimeSpan? SlidingExpiration
    {
        get => _cacheEntryImplementation.SlidingExpiration;
        set => _cacheEntryImplementation.SlidingExpiration = value;
    }

    public IList<IChangeToken> ExpirationTokens => _cacheEntryImplementation.ExpirationTokens;

    public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks => _cacheEntryImplementation.PostEvictionCallbacks;

    public CacheItemPriority Priority
    {
        get => _cacheEntryImplementation.Priority;
        set => _cacheEntryImplementation.Priority = value;
    }

    public long? Size
    {
        get => _cacheEntryImplementation.Size;
        set => _cacheEntryImplementation.Size = value;
    }
}