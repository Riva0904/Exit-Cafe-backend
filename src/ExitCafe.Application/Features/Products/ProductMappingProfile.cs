using AutoMapper;
using ExitCafe.Application.Features.Products.Dtos;
using ExitCafe.Domain.Entities;

namespace ExitCafe.Application.Features.Products;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductListItemDto>()
            .ForMember(d => d.PrimaryImageUrl, o => o.MapFrom(s => s.Images.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault() ?? s.Images.Select(i => i.ImageUrl).FirstOrDefault()))
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));

        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));

        CreateMap<ProductImage, ProductImageDto>();
    }
}
