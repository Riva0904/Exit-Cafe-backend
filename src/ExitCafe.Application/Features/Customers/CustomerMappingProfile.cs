using AutoMapper;
using ExitCafe.Application.Features.Customers.Dtos;
using ExitCafe.Domain.Entities;

namespace ExitCafe.Application.Features.Customers;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<CustomerAddress, CustomerAddressDto>();
        CreateMap<Customer, CustomerDto>()
            .ForMember(d => d.TotalOrders, o => o.MapFrom(s => s.Orders.Count))
            .ForMember(d => d.TotalSpent, o => o.MapFrom(s => s.Orders.Sum(x => x.TotalAmount)));
    }
}
