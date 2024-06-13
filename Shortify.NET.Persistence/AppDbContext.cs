using Microsoft.EntityFrameworkCore;

namespace Shortify.NET.Persistence
{
    public sealed class AppDbContext(DbContextOptions dbContextOptions) 
            : DbContext(dbContextOptions)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}
