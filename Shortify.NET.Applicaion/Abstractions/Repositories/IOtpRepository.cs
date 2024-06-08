namespace Shortify.NET.Applicaion.Abstractions.Repositories
{
    public interface IOtpRepository
    {
        Task AddOtpDetail(
                string email,
                string otp,
                DateTime otpRequestedOnUtc,
                DateTime otpExpiresOnUtc,
                CancellationToken cancellationToken = default);

        Task MarkOtpDetailAsUsed(
                Guid id,
                DateTime otpUsedOnUtc,
                CancellationToken cancellationToken = default);

        Task<(Guid, string)> GetLatestUnusedOtpAsync(
                                    string email,
                                    CancellationToken cancellationToken = default);
    }
}
