using Microsoft.Extensions.DependencyInjection;

namespace One.More.Lib.For.MediatR;

public partial class MediatRExtensionConfiguration
{
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
}