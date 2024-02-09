using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace One.More.Lib.For.MediatR.Test;

public class FluentValidationPipeBehaviorTest
{
    [Fact]
    public async Task Fluent_validation_pipeline_behavior_should_throw_an_exception_when_is_active_and_a_rule_is_not_respected()
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
    public async Task Fluent_validation_pipeline_behavior_should_not_throw_an_exception_when_is_active_and_rules_are_respected()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureFluentValidation()
            .BuildServiceProvider()
            .GetMediator();

        var sendHello = () => mediator.Send(new Hello("Pascal"));
        await sendHello.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Fluent_validation_pipeline_behavior_should_not_throw_an_exception_when_is_inactive_even_if_a_rule_is_not_respected()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureFluentValidation(active: false)
            .BuildServiceProvider()
            .GetMediator();

        var sendHello = () => mediator.Send(new Hello(""));
        await sendHello.Should().NotThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Fluent_validation_pipeline_behavior_should_accept_async_rules()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureFluentValidation()
            .BuildServiceProvider()
            .GetMediator();

        var sendHello = () => mediator.Send(new HelloAsync("Pascal"));
        await sendHello.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Fluent_validation_pipeline_behavior_should_throw_an_exception_when_an_async_rule_is_not_respected()
    {
        var mediator = ServiceCollectionBuilder.CreateServiceCollection()
            .ConfigureMediatR()
            .ConfigureFluentValidation()
            .BuildServiceProvider()
            .GetMediator();

        var sendHello = () => mediator.Send(new HelloAsync(HelloAsyncValidator.ForbiddenValue));
        var errors = (await sendHello.Should().ThrowAsync<ValidationException>())
            .And
            .Errors.ToList();

        errors.Count.Should().Be(1);
        errors[0].PropertyName.Should().Be(nameof(Hello.Message));
    }

}