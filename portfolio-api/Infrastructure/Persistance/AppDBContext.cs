using Microsoft.EntityFrameworkCore;
using portfolio_api.Domain.Entities;
using portfolio_api.Domain.Enums;

namespace portfolio_api.Infrastructure.Persistance
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<AuditLog> AuditLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<EventType>("event_type");

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("audit_log");
                entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
                entity.Property(e => e.EventType).HasColumnName("event_type").IsRequired().HasColumnType("event_type");
                entity.Property(e => e.Page).HasColumnName("page").HasMaxLength(500);
                entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(1000);
                entity.Property(e => e.MetaData).HasColumnName("meta_data").HasColumnType("jsonb");
                entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasColumnType("inet");
                entity.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(500);
                entity.Property(e => e.IsSuccess).HasColumnName("is_success").IsRequired().HasDefaultValueSql("true");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired().HasDefaultValueSql("now() at time zone 'utc'");
            });
        }

    }
}
