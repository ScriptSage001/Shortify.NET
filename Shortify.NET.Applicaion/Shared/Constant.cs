namespace Shortify.NET.Applicaion.Shared
{
    public static class Constant
    {
        /// <summary>
        /// Constants related to Emails
        /// </summary>
        public struct EmailConstants
        {
            /// <summary>
            /// Different OTP Types to send over Email
            /// </summary>
            public enum OtpType
            {
                VerifyEmail,
                ResetPassword,
                Login
            }

            /// <summary>
            /// Subject Lines for different Emails
            /// </summary>
            public struct Subject
            {
                public const string VerifyEmailOtp = "OTP for Email Verification";
                public const string ResetPasswordOtp = "OTP for Password Reset";
                public const string LoginOtp = "OTP for Login";
                public const string Welcome = "Welcome to Shortify.NET!";
                public const string PasswordChanged = "Your Password for Shortify.NET is changed";
            }
        }
    }
}
