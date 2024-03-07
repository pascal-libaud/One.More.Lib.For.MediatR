using FluentValidation;

namespace One.More.Lib.For.MediatR.Test;

public record VoidRequest(string Message) : IRequest;

public class VoidRequestHandler : IRequestHandler<VoidRequest>
{
    public Task Handle(VoidRequest request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public class VoidRequestValidator : AbstractValidator<VoidRequest>
{
    public VoidRequestValidator()
    {
        RuleFor(x => x.Message).NotNull().NotEmpty();
    }
}