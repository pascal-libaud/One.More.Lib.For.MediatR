using AsyncLinqR;
using FluentValidation;
using MediatR;

namespace One.More.Lib.For.MediatR;

internal class FluentValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public FluentValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var failures = await _validators.SelectAsync(v => v.ValidateAsync(request, cancellationToken))
            .SelectManyAsync(result => result.Errors)
            .WhereAsync(f => f != null, cancellationToken)
            .ToListAsync(cancellationToken);

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}