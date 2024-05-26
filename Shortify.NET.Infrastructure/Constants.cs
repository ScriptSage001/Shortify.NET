namespace Shortify.NET.Infrastructure
{
    public static class Constants
    {
        public struct AuthConstants
        {
            public struct ClaimType
            {
                public const string UserId = "UserId";
                public const string UserName = "UserName";
                public const string Email = "Email";
                public const string TokenType = "TokenType";
            }

            public struct ClaimTypeValue
            {
                public const string ResetPassword = "ResetPassword";
                public const string AccessToken = "AccessToken";
            }
        }
    }
}
