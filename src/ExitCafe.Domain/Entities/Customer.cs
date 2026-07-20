using ExitCafe.Domain.Common;

namespace ExitCafe.Domain.Entities;

public class Customer : BaseAuditableEntity
{
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public bool IsGuest { get; set; }

    public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
