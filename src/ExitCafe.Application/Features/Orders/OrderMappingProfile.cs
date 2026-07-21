using AutoMapper;
using ExitCafe.Application.Features.Orders.Dtos;
using ExitCafe.Domain.Entities;

namespace ExitCafe.Application.Features.Orders;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.Customer.FirstName + " " + s.Customer.LastName))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItems));
    }
}
