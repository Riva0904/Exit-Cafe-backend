using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Contact;

namespace ExitCafe.Application.Services.Interfaces;

public interface IContactService
{
    Task<ContactMessageDto> CreateAsync(CreateContactMessageRequest request, CancellationToken ct = default);
    Task<PagedResult<ContactMessageDto>> GetAllAsync(PaginationParams query, CancellationToken ct = default);
    Task<ContactMessageDto> MarkAsReadAsync(Guid id, CancellationToken ct = default);
}
