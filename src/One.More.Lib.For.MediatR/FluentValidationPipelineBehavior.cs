using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using One.More.Lib.For.Linq;
using MediatR;

namespace One.More.Lib.For.MediatR;

internal class FluentValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public FluentValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var failures = await _validators.OmSelectAsync(v => v.ValidateAsync(request, cancellationToken))
            .OmSelectManyAsync(result => result.Errors)
            .OmWhereAsync(f => f != null)
            .OmToListAsync();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}