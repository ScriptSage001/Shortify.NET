using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shortify.NET.Core.Entites;
using static Shortify.NET.Persistence.Constants.TableConstants;

namespace Shortify.NET.Persistence.Configurations
{
    internal sealed class UserRolesConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder
                .ToTable(TableNames.UserRole);

            builder
                .HasKey(ur => 
                    new { ur.UserId, ur.RoleId });

            builder
                .HasQueryFilter(ur => ur.RowStatus);
        }
    }
}