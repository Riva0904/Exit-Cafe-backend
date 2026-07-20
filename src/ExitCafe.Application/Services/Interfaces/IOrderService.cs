using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Orders;
using ExitCafe.Domain.Enums;

namespace ExitCafe.Application.Services.Interfaces;

public interface IOrderService
{
    Task<PagedResult<OrderDto>> GetAllAsync(PaginationParams query, OrderStatus? status, CancellationToken ct = default);
    Task<List<OrderDto>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default);
    Task<OrderDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken ct = default);
    Task<OrderDto> UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken ct = default);
    Task CancelAsync(Guid id, CancellationToken ct = default);
}
