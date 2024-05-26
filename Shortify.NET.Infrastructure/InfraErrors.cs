using Shortify.NET.Common.FunctionalTypes;

namespace Shortify.NET.Infrastructure
{
    internal static class InfraErrors
    {
        internal readonly struct Auth
        {
            public static readonly Error InvalidCredentials = Error.Unauthorized("Auth.InvalidCredentials", "Credentials passed are either wrong or expired.");
        }
    }
}
