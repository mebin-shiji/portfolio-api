using Microsoft.EntityFrameworkCore;
using portfolio_api.Features.AuditLog;

namespace portfolio_api.Infrastructure.Persistance
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        DbSet<AuditLog> AuditLog { get; set; }

    }
}
