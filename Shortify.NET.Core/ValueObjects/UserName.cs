using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Core.Errors;
using Shortify.NET.Core.Primitives;

namespace Shortify.NET.Core.ValueObjects
{
    public sealed class UserName : ValueObject
    {
        public const int MinLength = 5;

        public const int MaxLength = 20;

        private UserName(string value) => Value = value;

        public string Value { get; }

        public static Result<UserName> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<UserName>(DomainErrors.UserName.Empty);
            }

            if (value.Length > MaxLength)
            {
                return Result.Failure<UserName>(DomainErrors.UserName.TooLong);
            }

            if (value.Length < MinLength)
            {
                return Result.Failure<UserName>(DomainErrors.UserName.TooShort);
            }

            return new UserName(value);
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
