using Microsoft.Extensions.Caching.Memory;

namespace One.More.Lib.For.MediatR.Test;

public static class ServiceCollectionBuilder
{
    public static IServiceCollection CreateServiceCollection()
    {
        return new ServiceCollection();
    }

    public static IServiceCollection ConfigureMediatR(this IServiceCollection services)
    {
        return services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Hello>());
    }

    public static IServiceCollection ConfigureFluentValidation(this IServiceCollection services, bool active = true)
    {
        if (active)
            return services.AddMediatRExtensions(cfg => cfg.AddFluentValidationSupport(new[] { typeof(Hello).Assembly }));
        else
            return services;
    }

    public static IServiceCollection ConfigureMemoryCache(this IServiceCollection services, bool active = true)
    {
        if (active)
            return services.AddMediatRExtensions(cfg => cfg.AddMemoryCacheSupport());
        else
            return services;
    }

    public static IServiceCollection ConfigureMemoryCacheAndOverrideFor<T>(this IServiceCollection services)
    {
        return services.AddMediatRExtensions(cfg =>
        {
            cfg.AddMemoryCacheSupport();
            cfg.AddMemoryCacheSupportOverrideFor<T>(priority: CacheItemPriority.High);
        });
    }

    public static IServiceCollection ConfigureMemoryCacheDecorator(this IServiceCollection services)
    {
        services.AddOptions();
        services.AddSingleton<MemoryCache>();
        services.TryAddSingleton<IMemoryCache, SpyMemoryCache>();
        return services;
    }

    public static IServiceCollection ConfigurePerformanceLogger(this IServiceCollection services, bool active = true)
    {
        if (active)
            return services.AddMediatRExtensions(cfg => cfg.AddPerformanceLoggerSupport(100));
        else
            return services;
    }

    public static IServiceCollection ConfigureRetry(this IServiceCollection services, bool active = true)
    {
        if (active)
            return services.AddMediatRExtensions(cfg => cfg.AddRetrySupport(retryDelay: 50));
        else
            return services;
    }

    public static IMediator GetMediator(this ServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IMediator>();
    }

    public static ServiceProvider GetRequiredService<T>(this ServiceProvider serviceProvider, out T service) where T : notnull
    {
        service = serviceProvider.GetRequiredService<T>();
        return serviceProvider;
    }
}