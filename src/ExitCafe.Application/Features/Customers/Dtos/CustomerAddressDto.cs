namespace ExitCafe.Application.Features.Customers.Dtos;

public class CustomerAddressDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = default!;
    public string AddressLine1 { get; set; } = default!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public string Country { get; set; } = default!;
    public bool IsDefault { get; set; }
}
