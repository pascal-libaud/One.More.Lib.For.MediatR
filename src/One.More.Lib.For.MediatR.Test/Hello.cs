using FluentValidation;

namespace One.More.Lib.For.MediatR.Test;

public record Hello(string Message) : IRequest<string>;

public class HelloHandler : IRequestHandler<Hello, string>
{
    public async Task<string> Handle(Hello request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        return "Hello " + request.Message;
    }
}

public class HelloValidator : AbstractValidator<Hello>
{
    public HelloValidator()
    {
        RuleFor(x => x.Message).NotNull().NotEmpty();
    }
}