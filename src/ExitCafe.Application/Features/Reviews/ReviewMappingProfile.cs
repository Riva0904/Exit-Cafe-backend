using AutoMapper;
using ExitCafe.Application.Features.Reviews.Dtos;
using ExitCafe.Domain.Entities;

namespace ExitCafe.Application.Features.Reviews;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        CreateMap<Review, ReviewDto>();
    }
}
