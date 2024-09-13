using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shortify.NET.Core.Enums;
using Shortify.NET.Core.Entites;
using static Shortify.NET.Persistence.Constants.TableConstants;

namespace Shortify.NET.Persistence.Configurations
{
    internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder
                .ToTable(TableNames.Role, schema: TableSchema.MasterData);

            builder
                .HasKey(role => role.Id);

            SeedData(builder);            
        }

        private static void SeedData(EntityTypeBuilder<Role> builder)
        {
            var roleNames = Enum.GetNames(typeof(Roles));
            var roleId = 1;
            var roles = roleNames
                .Select<string, Role>(name => 
                    Role.Create(roleId++, name))
                .ToList();
                        
            builder.HasData(roles);
        }
    }
}