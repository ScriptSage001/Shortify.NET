﻿using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Core.Errors;
using Shortify.NET.Core.Primitives;

namespace Shortify.NET.Core.ValueObjects
{
    public sealed class Email : ValueObject
    {
        public const int MaxLength = 50;

        private Email(string value) => Value = value;

        public string Value { get; }

        public static Result<Email> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<Email>(DomainErrors.Email.Empty);
            }

            if (value.Split('@').Length != 2)
            {
                return Result.Failure<Email>(DomainErrors.Email.InvalidFormat);
            }

            return value.Length > MaxLength ? 
                Result.Failure<Email>(DomainErrors.Email.TooLong) : 
                new Email(value);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
