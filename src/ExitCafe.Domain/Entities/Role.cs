using ExitCafe.Domain.Common;

namespace ExitCafe.Domain.Entities;

public class Role : BaseAuditableEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
