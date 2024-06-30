namespace Shortify.NET.Common.FunctionalTypes
{
    public sealed class ValidationResult<TValue> : Result<TValue>, IValidationResult
    {
        private ValidationResult(Error[] errors)
            : base(default, false, IValidationResult.ValidationError) =>
            Errors = errors;

        public Error[] Errors { get; }

        public static ValidationResult<TValue> WithErrors(Error[] errors) => new(errors);

        public TNextValue Match<TNextValue>(Func<TValue, TNextValue> onSuccess, Func<Error[], TNextValue> onError)
        {
            return IsSuccess ? onSuccess(Value) : onError(Errors);
        }
    }
}
