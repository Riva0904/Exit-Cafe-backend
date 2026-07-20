using AutoMapper;
using ExitCafe.Application.DTOs.Categories;
using ExitCafe.Application.DTOs.Customers;
using ExitCafe.Application.DTOs.Orders;
using ExitCafe.Application.DTOs.Products;
using ExitCafe.Domain.Entities;

namespace ExitCafe.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.ProductCount, o => o.MapFrom(s => s.Products.Count));

        CreateMap<Product, ProductListItemDto>()
            .ForMember(d => d.PrimaryImageUrl, o => o.MapFrom(s => s.Images.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault() ?? s.Images.Select(i => i.ImageUrl).FirstOrDefault()))
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));

        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name));

        CreateMap<ProductImage, ProductImageDto>();

        CreateMap<CustomerAddress, CustomerAddressDto>();
        CreateMap<Customer, CustomerDto>()
            .ForMember(d => d.TotalOrders, o => o.MapFrom(s => s.Orders.Count))
            .ForMember(d => d.TotalSpent, o => o.MapFrom(s => s.Orders.Sum(x => x.TotalAmount)));

        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.Customer.FirstName + " " + s.Customer.LastName))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItems));
    }
}
