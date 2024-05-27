namespace Shortify.NET.Applicaion.Abstractions.Repositories
{
    public interface IOtpRepository
    {
        void AddOtpDetail(
                string email,
                string otp,
                DateTime otpRequestedOnUtc,
                DateTime otpExpiresOnUtc);

        void MarkOtpDetailAsUsed(
                Guid id,
                DateTime otpUsedOnUtc);
    }
}
