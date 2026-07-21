namespace ExitCafe.Application.Features.Customers.Dtos;

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
