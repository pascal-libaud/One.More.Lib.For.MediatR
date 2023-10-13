using System;
using System.Collections.Generic;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace One.More.Lib.For.MediatR
{
    public class MediatRExtensionConfiguration
    {
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

        public bool PerformanceSupport { get; set; } = false;

        public int TriggerThreshold { get; set; } = 0;

        public MediatRExtensionConfiguration AddPerformanceSupport(int triggerThreshold = 0)
        {
            PerformanceSupport = true;
            TriggerThreshold = triggerThreshold;
            return this;
        }

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

        internal bool FluentValidationSupport { get; set; } = false;
        
        internal IEnumerable<Assembly> Assemblies { get; private set; } = new List<Assembly>();

        internal Func<AssemblyScanner.AssemblyScanResult, bool> Filter { get; private set; } = null;
        
        internal bool IncludeInternalTypes { get; private set; } = false;

        public MediatRExtensionConfiguration AddFluentValidationSupport(IEnumerable<Assembly> assemblies, Func<AssemblyScanner.AssemblyScanResult, bool> filter = null, bool includeInternalTypes = false)
        {
            FluentValidationSupport = true;
            Assemblies = assemblies;
            Filter = filter;
            IncludeInternalTypes = includeInternalTypes;

            return this;
        }

        public bool RetrySupport { get; set; } = false;

        public int RetryCount { get; set; } = 5;

        public int RetryDelay { get; set; } = 500;

        public MediatRExtensionConfiguration AddRetrySupport(int? retryCount = null, int? retryDelay = null)
        {
            RetrySupport = true;
            RetryCount = retryCount ?? RetryCount;
            RetryDelay = retryDelay ?? RetryDelay;
            return this;
        }
    }
}
