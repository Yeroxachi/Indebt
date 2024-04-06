using Application.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers;

public class ApprovalMapperProfile : Profile
{
    public ApprovalMapperProfile()
    {
        CreateMap<MergeRequestApproval, MergeRequestApprovalResponse>(MemberList.Source);
    }
}