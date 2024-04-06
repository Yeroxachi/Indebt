using Application.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers;

public class MergeRequestMapperProfile : Profile
{
    public MergeRequestMapperProfile()
    {
        CreateMap<MergeRequest, MergeRequestResponse>(MemberList.Source)
            .ForMember(e => e.UserId, exp => { exp.MapFrom(dst => dst.InitiatorId); })
            .ForMember(e => e.MergeStatus, exp =>
            {
                exp.MapFrom(dst => dst.MergeStatus.ToString());
            })
            .ForMember(e => e.MergeGroups, exp =>
            {
                exp.MapFrom(dst=> dst.Groups.Select(x => x.GroupId).ToArray());
            });
}
}