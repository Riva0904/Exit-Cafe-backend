using AutoMapper;
using ExitCafe.Application.Features.Contact.Dtos;
using ExitCafe.Domain.Entities;

namespace ExitCafe.Application.Features.Contact;

public class ContactMappingProfile : Profile
{
    public ContactMappingProfile()
    {
        CreateMap<ContactMessage, ContactMessageDto>();
    }
}
