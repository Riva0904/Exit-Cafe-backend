namespace ExitCafe.Application.Features.Products.Dtos;

public class ProductImageDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = default!;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}
