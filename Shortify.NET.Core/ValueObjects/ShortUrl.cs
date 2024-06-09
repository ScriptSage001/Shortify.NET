using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Core.Errors;
using Shortify.NET.Core.Primitives;

namespace Shortify.NET.Core.ValueObjects
{
    public sealed class ShortUrl : ValueObject
    {
        private ShortUrl(string value) => Value = value;    

        public string Value { get; }

        public static Result<ShortUrl> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<ShortUrl>(DomainErrors.ShortUrl.Empty);
            }

            if (!Uri.TryCreate(value, UriKind.Absolute, out _))
            {
                return Result.Failure<ShortUrl>(DomainErrors.ShortUrl.InvalidFormat);
            }

            return new ShortUrl(value);
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
