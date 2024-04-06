using Application.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers;

public class GroupMergeConfirmationMapperProfile : Profile
{
    public GroupMergeConfirmationMapperProfile()
    {
        CreateMap<MergeRequestApproval, MergeRequestApprovalResponse>(MemberList.Source);
    }
}