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

        /// <summary>
        /// Error related to User Entity
        /// </summary>
        public readonly struct User
        {
            public static readonly Error UserNameAlreadyInUse = Error.Conflict("User.UserNameAlreadyInUse", "The specified UserName is already in use.");

            public static readonly Error EmailAlreadyInUse = Error.Conflict("User.EmailAlreadyInUse", "The specified Email is already in use.");

            public static readonly Error UserNotFound = Error.NotFound("User.UserNotFound", "The requested user doesn't Exists.");
        }

        /// <summary>
        /// Error related to UserCredentials Entity
        /// </summary>
        public readonly struct UserCredentials
        {
            public static readonly Error WrongCredentials = Error.Unauthorized("User.WrongCredentials", "The specified credentials are wrong.");
        }

        /// <summary>
        /// Error related to Otp
        /// </summary>
        public readonly struct Otp
        {
            public static readonly Error Invalid = Error.Unauthorized("Otp.Invalid", "The specified OTP is invalid.");
        }

        /// <summary>
        /// Error related to ShortenedUrl Entity
        /// </summary>
        public readonly struct ShortenedUrl
        {
            public static readonly Error ShortenedUrlNotFound = Error.NotFound("ShortenedUrl.NotFound", "The specified short url does not exists.");
        }
    }
}
