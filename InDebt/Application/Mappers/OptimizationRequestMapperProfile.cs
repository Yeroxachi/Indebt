using Application.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers;

public class OptimizationRequestMapperProfile : Profile
{
    public OptimizationRequestMapperProfile()
    {
        CreateMap<OptimizationRequest, OptimizationResponse>(MemberList.Source)
            .ForMember(d => d.Status, opt => opt.MapFrom(o => o.Status.ToString()));
    }
}