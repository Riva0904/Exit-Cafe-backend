namespace ExitCafe.Application.DTOs.Customers;

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

public record CreateAddressRequest(string Label, string AddressLine1, string? AddressLine2, string City, string State, string PostalCode, string Country, bool IsDefault);

public class CustomerDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public bool IsGuest { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
    public List<CustomerAddressDto> Addresses { get; set; } = new();
}
