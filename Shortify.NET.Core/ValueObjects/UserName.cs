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

            return value.Length > MaxLength ? Result.Failure<UserName>(DomainErrors.UserName.TooLong) :
                value.Length < MinLength ? Result.Failure<UserName>(DomainErrors.UserName.TooShort) :
                new UserName(value);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
