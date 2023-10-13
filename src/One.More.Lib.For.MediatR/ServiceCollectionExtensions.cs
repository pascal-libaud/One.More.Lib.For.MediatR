using System;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace One.More.Lib.For.MediatR
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatRExtensions(this IServiceCollection services, Action<MediatRExtensionConfiguration> configuration)
        {
            var serviceConfig = new MediatRExtensionConfiguration();

            configuration(serviceConfig);

            if (serviceConfig.PerformanceSupport)
            {
                services.AddSingleton(new PerformancePipelineBehaviorConfiguration { TriggerThreshold = serviceConfig.TriggerThreshold });

                services.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), typeof(PerformancePipelineBehavior<,>), serviceConfig.Lifetime));
            }

            if (serviceConfig.MemoryCacheSupport)
            {
                services.AddMemoryCache();

                services.AddSingleton(new MemoryCachePipelineBehaviorConfiguration
                {
                    AbsoluteExpiration = serviceConfig.AbsoluteExpiration,
                    AbsoluteExpirationRelativeToNow = serviceConfig.AbsoluteExpirationRelativeToNow,
                    Priority = serviceConfig.Priority,
                    SlidingExpiration = serviceConfig.SlidingExpiration,
                });

                services.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), typeof(MemoryCachePipelineBehavior<,>), serviceConfig.Lifetime));
            }

            if (serviceConfig.FluentValidationSupport)
            {
                services.AddValidatorsFromAssemblies(serviceConfig.Assemblies, serviceConfig.Lifetime, serviceConfig.Filter, serviceConfig.IncludeInternalTypes);

                services.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), typeof(FluentValidationPipelineBehavior<,>), serviceConfig.Lifetime));
            }

            if (serviceConfig.RetrySupport)
            {
                services.AddSingleton(new RetryPipelineBehaviorConfiguration
                {
                    RetryCount = serviceConfig.RetryCount,
                    RetryDelay = serviceConfig.RetryDelay
                });

                services.Add(new ServiceDescriptor(typeof(IPipelineBehavior<,>), typeof(RetryPipelineBehavior<,>), serviceConfig.Lifetime));
            }

            return services;
        }
    }
}