using Microsoft.Extensions.DependencyInjection;

namespace One.More.Lib.For.MediatR.Test;

public class BasicConfigurationTest
{
    [Fact]
    public async Task MediatR_basic_configuration_should_work_well()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureFluentValidation()
            .BuildServiceProvider()
            .GetMediator();

        Assert.Equal("Hello World", await mediator.Send(new Hello("World")));
    }
}