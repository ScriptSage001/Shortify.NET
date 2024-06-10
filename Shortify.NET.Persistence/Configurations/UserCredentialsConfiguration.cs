using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shortify.NET.Core.Entites;
using static Shortify.NET.Persistence.Constants.TableConstants;

namespace Shortify.NET.Persistence.Configurations
{
    internal sealed class UserCredentialsConfiguration : IEntityTypeConfiguration<UserCredentials>
    {
        public void Configure(EntityTypeBuilder<UserCredentials> builder)
        {
            builder.ToTable(TableNames.UserCredentials);

            builder.HasKey(creds => creds.Id);

            builder
                .HasOne<User>()
                .WithOne(user => user.UserCredentials)
                .HasForeignKey<UserCredentials>(creds => creds.UserId);

            builder
                .HasQueryFilter(creds => creds.RowStatus);
        }
    }
}
