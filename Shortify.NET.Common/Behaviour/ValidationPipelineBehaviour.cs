using FluentValidation;
using MediatR;
using Shortify.NET.Common.FunctionalTypes;

namespace Shortify.NET.Common.Behaviour
{
    internal sealed class ValidationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<Result>
        where TResponse : Result
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            Error[] errors = _validators
                                .Select(validator => validator.Validate(request))
                                .SelectMany(validationResult => validationResult.Errors)
                                .Where(failure => failure is not null)
                                .Select(failure => Error.Validation(
                                                        failure.PropertyName,
                                                        failure.ErrorMessage))
                                .Distinct()
                                .ToArray();

            if (errors.Length != 0)
            {
                return CreateValidationResult<TResponse>(errors);
            }

            return await next();
        }

        /// <summary>
        /// To Create ValidationResut Object
        /// as Result or as Result<>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="errors"></param>
        /// <returns></returns>
        private TResult CreateValidationResult<TResult>(Error[] errors)
            where TResult : Result
        {
            if (typeof(TResult) == typeof(Result))
            {
                return (ValidationResult.WithErrors(errors) as TResult)!;
            }

            object validationResult = typeof(ValidationResult<>)
                                    .GetGenericTypeDefinition()
                                    .MakeGenericType(typeof(TResult).GenericTypeArguments[0])
                                    .GetMethod(nameof(ValidationResult.WithErrors))!
                                    .Invoke(null, [errors])!;

            return (TResult)validationResult;
        }
    }
}
