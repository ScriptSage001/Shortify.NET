namespace Shortify.NET.Common.FunctionalTypes
{
    /// <summary>
    /// Class for Defining Result Type
    /// </summary>
    public class Result
    {
        #region Constructor

        /// <summary>
        /// Constructor for Result initialization
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="error"></param>
        /// <exception cref="InvalidOperationException"></exception>
        protected internal Result(bool isSuccess, Error error)
        {
            if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            {
                throw new InvalidOperationException("Invalid Result Scenario.");
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        #endregion

        #region Properties

        /// <summary>
        /// States Success Result
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// States Failure Result
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Carries Error incase of Failure
        /// </summary>
        public Error Error { get; }

        #endregion

        #region Methods

        public static Result Success() => new(true, Error.None);

        /// <summary>
        /// Genric Success
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

        public static Result Failure(Error error) => new(false, error);

        /// <summary>
        /// Genric Failure
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

        /// <summary>
        /// Creates a Success Result if the Condition Passed is true
        /// Otherwise Creates a Failure Result with ConditionNotMet Error
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static Result Create(bool condition) => condition ?
                                                        Success() : Failure(Error.ConditionNotMet);

        /// <summary>
        /// Creates a Success Result with the Value if the value is not null
        /// Otherwise Creates a Failure Result with NullValue Error
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Result<TValue> Create<TValue>(TValue? value) => value is not null ?
                                                                        Success(value) : Failure<TValue>(Error.NullValue);

        #endregion
    }
}
