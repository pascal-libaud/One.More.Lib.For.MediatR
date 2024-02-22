using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace One.More.Lib.For.MediatR.Test;

public static class ServiceCollectionBuilder
{
    public static IServiceCollection CreateServiceCollection()
    {
        return new ServiceCollection();
    }

    public static IServiceCollection ConfigureMediatR(this IServiceCollection services)
    {
        return services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Hello>());
    }

    public static IServiceCollection ConfigureFluentValidation(this IServiceCollection services, bool active = true)
    {
        if (active)
            return services
                .AddMediatRExtensions(cfg => cfg.AddFluentValidationSupport(new[] { typeof(Hello).Assembly }));
        else
            return services;
    }

    public static IMediator GetMediator(this ServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IMediator>();
    }
}