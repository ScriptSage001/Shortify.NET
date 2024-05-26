using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shortify.NET.Core.Entites;
using Shortify.NET.Core.ValueObjects;
using static Shortify.NET.Persistence.Constants.TableConstants;

namespace Shortify.NET.Persistence.Configurations
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(TableNames.Users);

            builder.HasKey(user => user.Id);

            builder
                .Property(user => user.UserName)
                .HasConversion(
                    userName => userName.Value,
                    value => UserName.Create(value).Value)
                .HasMaxLength(UserName.MaxLength);

            builder
                .Property(user => user.Email)
                .HasConversion(
                    email => email.Value,
                    value => Email.Create(value).Value)
                .HasMaxLength(Email.MaxLength);

            builder
                .HasIndex(user => user.UserName)
                .IsUnique();

            builder
                .HasIndex(user => user.Email)
                .IsUnique();

            builder
                .HasOne(user => user.UserCredentials)
                .WithOne()
                .HasForeignKey<UserCredentials>(userCred => userCred.UserId);

            builder
                .HasMany(user => user.ShortenedUrls)
                .WithOne()
                .HasForeignKey(shortenedUrl => shortenedUrl.UserId);

            builder
                .HasQueryFilter(user => user.RowStatus);
        }
    }
}
