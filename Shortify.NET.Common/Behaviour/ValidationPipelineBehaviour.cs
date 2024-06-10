using FluentValidation;
using MediatR;
using Shortify.NET.Common.FunctionalTypes;

namespace Shortify.NET.Common.Behaviour
{
    /// <summary>
    /// Represents a validation behavior in a pipeline for processing requests and responses.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request, which must implement <see cref="IRequest{Result}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response, which must be a subclass of <see cref="Result"/>.</typeparam>
    internal sealed class ValidationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<Result>
        where TResponse : Result
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationPipelineBehaviour{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="validators">The collection of validators for the request type.</param>
        public ValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <summary>
        /// Handles the request validation and delegates to the next handler in the pipeline.
        /// </summary>
        /// <param name="request">The request to be validated.</param>
        /// <param name="next">The next handler delegate in the pipeline.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation, containing the response.</returns>
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
        /// Creates a <see cref="ValidationResult"/> object either as a <see cref="Result"/>
        /// or as a <see cref="Result{T}"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result, which must be a subclass of <see cref="Result"/>.</typeparam>
        /// <param name="errors">An array of errors to include in the validation result.</param>
        /// <returns>A <typeparamref name="TResult"/> object containing the specified errors.</returns>
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
