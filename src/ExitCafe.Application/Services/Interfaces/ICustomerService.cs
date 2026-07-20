using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Customers;

namespace ExitCafe.Application.Services.Interfaces;

public interface ICustomerService
{
    Task<PagedResult<CustomerDto>> GetAllAsync(PaginationParams query, CancellationToken ct = default);
    Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CustomerAddressDto> AddAddressAsync(Guid customerId, CreateAddressRequest request, CancellationToken ct = default);
    Task<List<CustomerAddressDto>> GetAddressesAsync(Guid customerId, CancellationToken ct = default);
    Task DeleteAddressAsync(Guid customerId, Guid addressId, CancellationToken ct = default);
}
