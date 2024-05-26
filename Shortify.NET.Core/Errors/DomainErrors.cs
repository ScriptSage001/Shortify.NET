using Shortify.NET.Common.FunctionalTypes;

namespace Shortify.NET.Core.Errors
{
    /// <summary>
    /// Predefined Domain Errors
    /// </summary>
    public static class DomainErrors
    {
        /// <summary>
        /// Domain Errors related to UserName Value Object
        /// </summary>
        public readonly struct UserName
        {
            public static readonly Error Empty = Error.Validation("UserName.Empty", "UserName is empty");

            public static readonly Error TooLong = Error.Validation("UserName.TooLong", "UserName is too long");

            public static readonly Error TooShort = Error.Validation("UserName.TooShort", "UserName is too short");
        }

        /// <summary>
        /// Domain Errors related to Email Value Object
        /// </summary>
        public readonly struct Email
        {
            public static readonly Error Empty = Error.Validation("Email.Empty", "Email is empty");

            public static readonly Error InvalidFormat = Error.Validation("Email.InvalidFormat", "Email format is invalid");

            public static readonly Error TooLong = Error.Validation("Email.TooLong", "Email is too long");
        }

        /// <summary>
        /// Domain Errors related to ShortUrl Value Object
        /// </summary>
        public readonly struct ShortUrl
        {
            public static readonly Error Empty = Error.Validation("ShortUrl.Empty", "ShortUrl is empty");

            public static readonly Error InvalidFormat = Error.Validation("ShortUrl.InvalidFormat", "ShortUrl format is invalid");
        }
    }
}
