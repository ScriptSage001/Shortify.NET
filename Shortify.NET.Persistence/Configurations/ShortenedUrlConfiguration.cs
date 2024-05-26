using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shortify.NET.Core.Entites;
using static Shortify.NET.Persistence.Constants.TableConstants;

namespace Shortify.NET.Persistence.Configurations
{
    internal sealed class ShortenedUrlConfiguration : IEntityTypeConfiguration<ShortenedUrl>
    {
        public void Configure(EntityTypeBuilder<ShortenedUrl> builder)
        {
            builder.ToTable(TableNames.ShortenedUrls);

            builder
                .Property(shortenedUrl => shortenedUrl.Code)
                .HasMaxLength(7);

            builder
                .HasIndex(shortenedUrl => shortenedUrl.Code)
                .IsUnique();
        }
    }
}
