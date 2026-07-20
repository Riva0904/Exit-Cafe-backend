using ExitCafe.Domain.Common;

namespace ExitCafe.Domain.Entities;

public class CustomerAddress : BaseAuditableEntity
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public string Label { get; set; } = default!;
    public string AddressLine1 { get; set; } = default!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public string Country { get; set; } = "India";
    public bool IsDefault { get; set; }
}
