using Microsoft.EntityFrameworkCore;
using portfolio_api.Features.AuditLog;

namespace portfolio_api.Infrastructure.Persistance
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<AuditLog> AuditLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Action).IsRequired().HasConversion<string>();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
                entity.Property(e => e.Data).HasColumnType("jsonb");
                entity.Property(e => e.IpAddress).HasMaxLength(45);
            });
        }

    }
}
