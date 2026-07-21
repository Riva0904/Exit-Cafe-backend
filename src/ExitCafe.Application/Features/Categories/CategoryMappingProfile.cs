using AutoMapper;
using ExitCafe.Application.Features.Categories.Dtos;
using ExitCafe.Domain.Entities;

namespace ExitCafe.Application.Features.Categories;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.ProductCount, o => o.MapFrom(s => s.Products.Count));
    }
}
