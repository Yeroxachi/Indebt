using Application.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers;

public class OptimizationApprovalMapperProfile : Profile
{
    public OptimizationApprovalMapperProfile()
    {
        CreateMap<OptimizationRequestApproval, OptimizationApprovalResponse>(MemberList.Source);
    }
}