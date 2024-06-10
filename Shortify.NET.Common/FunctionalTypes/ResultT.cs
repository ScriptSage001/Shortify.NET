namespace Shortify.NET.Common.FunctionalTypes
{
    /// <summary>
    /// Class for Defining Genric Result Type
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        protected internal Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
        {
            _value = value;
        }

        public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("The Value of the Failure Result cannot be accessed.");

        public static implicit operator Result<TValue>(TValue? value) => Create(value);

        public TNextValue Match<TNextValue>(Func<TValue, TNextValue> onSuccess, Func<Error, TNextValue> onError)
        {
            if (IsSuccess)
            {
                return onSuccess(Value);
            }

            return onError(Error);
        }
    }
}
