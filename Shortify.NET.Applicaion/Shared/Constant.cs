namespace Shortify.NET.Applicaion.Shared
{
    public static class Constant
    {
        #region  EmailConstants

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

        #endregion
        
        #region  Cache Constants

        /// <summary>
        /// Constants related to Cache
        /// </summary>
        public struct Cache
        {
            /// <summary>
            /// Cache Key Prefixes
            /// </summary>
            public struct Prefixes
            {
                public const string OriginalUrls = "Original_Url_";
                public const string RoleByName = "Role_By_Name_";
                public const string RoleById = "Role_By_Id_";
            }
        }

        #endregion
    }
}
