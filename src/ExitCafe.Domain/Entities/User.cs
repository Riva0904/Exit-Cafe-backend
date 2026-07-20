using ExitCafe.Domain.Common;

namespace ExitCafe.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public Customer? Customer { get; set; }
}
