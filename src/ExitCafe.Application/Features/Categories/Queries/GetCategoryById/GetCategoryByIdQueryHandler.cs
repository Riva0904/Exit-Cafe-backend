using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Categories.Dtos;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        var category = await _uow.Categories.Query().Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct)
            ?? throw new NotFoundException(nameof(Category), request.Id);
        return _mapper.Map<CategoryDto>(category);
    }
}
