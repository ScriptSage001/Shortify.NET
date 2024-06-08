using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shortify.NET.Core.Entites;
using Shortify.NET.Core.ValueObjects;
using static Shortify.NET.Persistence.Constants.TableConstants;

namespace Shortify.NET.Persistence.Configurations
{
    internal sealed class ShortenedUrlConfiguration : IEntityTypeConfiguration<ShortenedUrl>
    {
        public void Configure(EntityTypeBuilder<ShortenedUrl> builder)
        {
            builder.ToTable(TableNames.ShortenedUrls);

            builder
                .Property(shortenedUrl => shortenedUrl.ShortUrl)
                .HasConversion(
                    shortUrl => shortUrl.Value,
                    value => ShortUrl.Create(value).Value);

            builder
                .Property(shortenedUrl => shortenedUrl.Code)
                .HasMaxLength(7);

            builder
                .HasIndex(shortenedUrl => shortenedUrl.Code)
                .IsUnique();

            builder
                .HasQueryFilter(shortenedUrl => shortenedUrl.RowStatus);
        }
    }
}
