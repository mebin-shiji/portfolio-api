using portfolio_api.Features.AuditLog;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace portfolio_api.Data.Entities
{
    public class AuditLog
    {
        [Key]
        public int Id { get; init; }
        public AuditType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public JsonDocument? Data { get; set; }
    }
}
