using Shortify.NET.Core.Primitives;

namespace Shortify.NET.Core.Entites
{
    /// <summary>
    /// UserOtpDetails Entity Class
    /// </summary>
    public sealed class UserOtpDetails : Entity
    {
        #region Constructor

        /// <summary>
        /// Constructor to initailize UserOtpDetails entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="otp"></param>
        /// <param name="otpExpiresOnUtc"></param>
        private UserOtpDetails(Guid id, string email, string otp, DateTime otpExpiresOnUtc)
            : base(id)
        {
            Email = email;
            Otp = otp;
            IsUsed = false;
            OtpRequestedOnUtc = DateTime.UtcNow;
            OtpExpiresOnUtc = otpExpiresOnUtc;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// OTP
        /// </summary>
        public string Otp { get; private set; }

        /// <summary>
        /// To check if the OTP is already used
        /// </summary>
        public bool IsUsed { get; private set; }

        /// <summary>
        /// OTP Requested On UTC
        /// </summary>
        public DateTime OtpRequestedOnUtc { get; private set; }

        /// <summary>
        /// OTP Expires On UTC
        /// </summary>
        public DateTime OtpExpiresOnUtc { get; private set; }

        /// <summary>
        /// OTP Used On UTC
        /// </summary>
        public DateTime? OtpUsedOnUtc { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// To Create New UserOtpDetails
        /// </summary>
        /// <param name="email"></param>
        /// <param name="otp"></param>
        /// <param name="otpExpiresOnUtc"></param>
        /// <returns></returns>
        public static UserOtpDetails Create(string email, string otp, DateTime otpExpiresOnUtc)
        {
            return new UserOtpDetails(Guid.NewGuid(), email, otp, otpExpiresOnUtc);
        }

        /// <summary>
        /// To make one OTP used
        /// </summary>
        public void OtpUsed()
        {
            IsUsed = true;
            OtpUsedOnUtc = DateTime.UtcNow;
        }

        #endregion
    }
}
