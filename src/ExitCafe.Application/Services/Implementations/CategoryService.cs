using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Utilities;
using ExitCafe.Application.DTOs.Categories;
using ExitCafe.Application.Services.Interfaces;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IAuditLogService _auditLog;

    public CategoryService(IUnitOfWork uow, IMapper mapper, IAuditLogService auditLog)
    {
        _uow = uow;
        _mapper = mapper;
        _auditLog = auditLog;
    }

    public async Task<List<CategoryDto>> GetAllAsync(bool includeInactive, CancellationToken ct = default)
    {
        IQueryable<Category> query = _uow.Categories.Query().Include(c => c.Products);
        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        var categories = await query.OrderBy(c => c.DisplayOrder).ToListAsync(ct);
        return _mapper.Map<List<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _uow.Categories.Query().Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException(nameof(Category), id);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var category = await _uow.Categories.Query().Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Slug == slug, ct)
            ?? throw new NotFoundException(nameof(Category), slug);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        var slug = SlugHelper.GenerateSlug(request.Name);
        if (await _uow.Categories.AnyAsync(c => c.Slug == slug, ct))
            throw new ConflictException($"Category with slug '{slug}' already exists.");

        var category = new Category
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            DisplayOrder = request.DisplayOrder,
            ParentCategoryId = request.ParentCategoryId,
            IsActive = true
        };

        await _uow.Categories.AddAsync(category, ct);
        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken ct = default)
    {
        var category = await _uow.Categories.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Category), id);

        category.Name = request.Name;
        category.Slug = SlugHelper.GenerateSlug(request.Name);
        category.Description = request.Description;
        category.ImageUrl = request.ImageUrl;
        category.DisplayOrder = request.DisplayOrder;
        category.IsActive = request.IsActive;
        category.ParentCategoryId = request.ParentCategoryId;

        _uow.Categories.Update(category);
        await _uow.SaveChangesAsync(ct);

        var updated = await _uow.Categories.Query().Include(c => c.Products).FirstAsync(c => c.Id == category.Id, ct);
        return _mapper.Map<CategoryDto>(updated);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _uow.Categories.Query().Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException(nameof(Category), id);

        // A soft-deleted category is excluded by its own query filter, which silently drops any
        // product still assigned to it out of Include()-based joins (count queries still see them,
        // list queries don't — a confusing, hard-to-spot data gap). Deactivate instead of blocking
        // outright would still hide products from customers, so require the category to be emptied
        // (or its products reassigned) first.
        if (category.Products.Any(p => !p.IsDeleted))
            throw new ConflictException(
                $"Cannot delete '{category.Name}': it still has {category.Products.Count(p => !p.IsDeleted)} product(s). Reassign or remove them first.");

        category.IsDeleted = true;
        category.IsActive = false;
        _uow.Categories.Update(category);
        await _uow.SaveChangesAsync(ct);
        await _auditLog.LogAsync("CategoryDeleted", nameof(Category), category.Id.ToString(), ct: ct);
    }
}
