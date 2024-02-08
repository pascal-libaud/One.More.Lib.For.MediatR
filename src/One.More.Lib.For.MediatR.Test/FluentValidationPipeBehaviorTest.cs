using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace One.More.Lib.For.MediatR.Test;

public class FluentValidationPipeBehaviorTest
{
    [Fact]
    public async Task Mediator_should_throw_an_exception_when_fluent_validation_pipeline_behavior_is_active()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureFluentValidation()
            .BuildServiceProvider()
            .GetMediator();

        var sendHello = () => mediator.Send(new Hello(""));
        var errors = (await sendHello.Should().ThrowAsync<ValidationException>())
            .And
            .Errors.ToList();

        errors.Count.Should().Be(1);
        errors[0].PropertyName.Should().Be(nameof(Hello.Message));
    }

    [Fact]
    public async Task Mediator_should_not_throw_an_exception_when_fluent_validation_pipeline_behavior_is_inactive()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureFluentValidation(false)
            .BuildServiceProvider()
            .GetMediator();

        var sendHello = () => mediator.Send(new Hello(""));
        await sendHello.Should().NotThrowAsync<ValidationException>();
    }
}