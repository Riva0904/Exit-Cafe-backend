using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.CustomCakeOrders;
using ExitCafe.Domain.Enums;

namespace ExitCafe.Application.Services.Interfaces;

public interface ICustomCakeOrderService
{
    Task<CustomCakeOrderDto> CreateAsync(CreateCustomCakeOrderRequest request, CancellationToken ct = default);
    Task<PagedResult<CustomCakeOrderDto>> GetAllAsync(PaginationParams query, CustomCakeOrderStatus? status, CancellationToken ct = default);
    Task<CustomCakeOrderDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CustomCakeOrderDto> UpdateStatusAsync(Guid id, CustomCakeOrderStatus status, CancellationToken ct = default);
}
