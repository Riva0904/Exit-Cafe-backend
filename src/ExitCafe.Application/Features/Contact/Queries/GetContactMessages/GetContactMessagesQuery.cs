using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Contact.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Contact.Queries.GetContactMessages;

public class GetContactMessagesQuery : PaginationParams, IRequest<PagedResult<ContactMessageDto>>
{
}
