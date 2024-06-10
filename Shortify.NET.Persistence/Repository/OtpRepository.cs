using Microsoft.EntityFrameworkCore;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Persistence.Models;

namespace Shortify.NET.Persistence.Repository
{
    public class OtpRepository(AppDbContext appDbContext) 
        : IOtpRepository
    {
        private readonly AppDbContext _appDbContext = appDbContext;

        public async Task AddOtpDetail(
            string email, 
            string otp, 
            DateTime otpRequestedOnUtc, 
            DateTime otpExpiresOnUtc,
            CancellationToken cancellationToken = default)
        {
            OtpDetails otpDetails = new()
            {
                Id = Guid.NewGuid(),
                Email = email,
                Otp = otp,
                OtpRequestedOnUtc = otpRequestedOnUtc,
                OtpExpiresOnUtc = otpExpiresOnUtc,
                IsUsed = false,
                OtpUsedOnUtc = null
            };

            await _appDbContext
                        .Set<OtpDetails>()
                        .AddAsync(otpDetails, cancellationToken);
        }

        public async Task MarkOtpDetailAsUsed(
            Guid id, 
            DateTime otpUsedOnUtc,
            CancellationToken cancellationToken = default)
        {
            var otp = await _appDbContext
                                .Set<OtpDetails>()
                                .FindAsync(
                                        [id], 
                                        cancellationToken);

            if (otp != null)
            {
                otp.IsUsed = true;
                otp.OtpUsedOnUtc = otpUsedOnUtc;
            }
        }

        public async Task<(Guid, string)> GetLatestUnusedOtpAsync(string email, CancellationToken cancellationToken = default)
        {
            var otp = await _appDbContext
                                .Set<OtpDetails>()
                                .AsNoTracking()
                                .OrderByDescending(otp => otp.OtpRequestedOnUtc)
                                .FirstOrDefaultAsync(
                                        otp => 
                                               otp.Email == email
                                            && !otp.IsUsed
                                            && otp.OtpExpiresOnUtc >= DateTime.UtcNow,
                                        cancellationToken);

            return otp is not null 
                        ? (otp.Id, otp.Otp) 
                        : (Guid.Empty, string.Empty);
        }
    }
}
