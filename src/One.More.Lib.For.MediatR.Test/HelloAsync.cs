using FluentValidation;
using MediatR;

namespace One.More.Lib.For.MediatR.Test;

public record HelloAsync(string Message) : IRequest<string>;

public class HelloAsyncHandler : IRequestHandler<HelloAsync, string>
{
    public async Task<string> Handle(HelloAsync request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        return "Hello " + request.Message;
    }
}

public class HelloAsyncValidator : AbstractValidator<HelloAsync>
{
    public const string ForbiddenValue = "Toto";

    public HelloAsyncValidator()
    {
        RuleFor(x => x.Message)
            .NotNull()
            .NotEmpty()
            .MustAsync(async (message, _) =>
            {
                await Task.Yield();
                return message != ForbiddenValue;
            });
    }
}

