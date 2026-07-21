using AutoMapper;
using ExitCafe.Application.Features.Notifications.Dtos;
using ExitCafe.Domain.Entities;

namespace ExitCafe.Application.Features.Notifications;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<Notification, NotificationDto>();
    }
}
