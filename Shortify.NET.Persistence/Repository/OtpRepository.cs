using Microsoft.EntityFrameworkCore;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Persistence.Models;

namespace Shortify.NET.Persistence.Repository
{
    public class OtpRepository : IOtpRepository
    {
        private readonly AppDbContext _appDbContext;

        public OtpRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void AddOtpDetail(
            string email, 
            string otp, 
            DateTime otpRequestedOnUtc, 
            DateTime otpExpiresOnUtc)
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

            _appDbContext
                    .Set<OtpDetails>()
                    .Add(otpDetails);
        }

        public void MarkOtpDetailAsUsed(Guid id, DateTime otpUsedOnUtc)
        {
            var otp = new OtpDetails
            {
                Id = id,
                IsUsed = true,
                OtpUsedOnUtc = otpUsedOnUtc
            };

            _appDbContext.Attach(otp);
            
            _appDbContext.Entry(otp).Property(x => x.IsUsed).IsModified = true;
            _appDbContext.Entry(otp).Property(x => x.OtpUsedOnUtc).IsModified = true;
        }

        public async Task<(Guid, string)> GetLatestUnusedOtpAsync(string email)
        {
            var otp = await _appDbContext
                                .Set<OtpDetails>()
                                .AsNoTracking()
                                .FirstOrDefaultAsync(
                                        otp => 
                                               otp.Email == email
                                            && otp.IsUsed == false
                                            && otp.OtpExpiresOnUtc >= DateTime.UtcNow);

            return otp is not null 
                        ? (otp.Id, otp.Otp) 
                        : (Guid.Empty, string.Empty);
        }
    }
}
