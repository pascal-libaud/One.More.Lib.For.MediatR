using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Polly;

namespace One.More.Lib.For.MediatR
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class RetryPolicyAttribute : Attribute
    {
        public int? RetryCount { get; set; }
        public int? RetryDelay { get; set; }
    }

    internal class RetryConfiguration
    {
        internal int RetryCount { get; set; }
        internal int RetryDelay { get; set; }
    }

    internal class RetryPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<RetryPipelineBehavior<TRequest, TResponse>> _logger;
        private readonly RetryConfiguration _configuration;

        public RetryPipelineBehavior(ILogger<RetryPipelineBehavior<TRequest, TResponse>> logger, RetryConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var retryPolicy = typeof(TRequest).GetCustomAttribute<RetryPolicyAttribute>();

            if (retryPolicy == null)
                return await next();

            Func<int, TimeSpan> sleepDurationProvider = i => TimeSpan.FromMilliseconds(i * (retryPolicy.RetryDelay ?? _configuration.RetryDelay));

            return await Policy.Handle<Exception>()
                .WaitAndRetryAsync(retryPolicy.RetryCount ?? _configuration.RetryCount, sleepDurationProvider, OnRetry)
                .ExecuteAsync(async _ => await next(), cancellationToken);
        }

        private void OnRetry(Exception exception, TimeSpan timespan, Context _)
        {
            string message = $"Failed to execute handler for {typeof(TRequest).Name}, retrying after {timespan.TotalSeconds}s ({exception.Message})";
            _logger.LogWarning(exception, message);
        }
    }
}