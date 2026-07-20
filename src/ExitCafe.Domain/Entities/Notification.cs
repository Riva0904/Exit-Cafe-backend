using ExitCafe.Domain.Common;

namespace ExitCafe.Domain.Entities;

public class Notification : BaseEntity
{
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string Type { get; set; } = default!;
    public Guid? RelatedEntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
