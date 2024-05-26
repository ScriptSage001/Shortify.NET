using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shortify.NET.Persistence.Models;
using static Shortify.NET.Persistence.Constants.TableConstants;

namespace Shortify.NET.Persistence.Configurations
{
    internal sealed class OtpDetailsConfiguration : IEntityTypeConfiguration<OtpDetails>
    {
        public void Configure(EntityTypeBuilder<OtpDetails> builder)
        {
            builder.ToTable(TableNames.OtpDetails);

            builder.HasKey(otp => otp.Id);

            builder
                .Property(otp => otp.Otp)
                .HasMaxLength(OtpDetails.OtpMaxLength);

            builder
                .Property(otp => otp.Email)
                .HasMaxLength(OtpDetails.EmailMaxLength);
        }
    }
}
