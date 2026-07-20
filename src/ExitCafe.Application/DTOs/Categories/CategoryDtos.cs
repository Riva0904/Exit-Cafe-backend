namespace ExitCafe.Application.DTOs.Categories;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int ProductCount { get; set; }
}

public record CreateCategoryRequest(string Name, string? Description, string? ImageUrl, int DisplayOrder, Guid? ParentCategoryId);

public record UpdateCategoryRequest(string Name, string? Description, string? ImageUrl, int DisplayOrder, bool IsActive, Guid? ParentCategoryId);
