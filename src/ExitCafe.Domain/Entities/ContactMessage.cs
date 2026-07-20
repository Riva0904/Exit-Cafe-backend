using ExitCafe.Domain.Common;

namespace ExitCafe.Domain.Entities;

public class ContactMessage : BaseAuditableEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public string Subject { get; set; } = default!;
    public string Message { get; set; } = default!;
    public bool IsRead { get; set; }
}
