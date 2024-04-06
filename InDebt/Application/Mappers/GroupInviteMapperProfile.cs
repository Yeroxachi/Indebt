using Application.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers;

public class GroupInviteMapperProfile : Profile
{
    public GroupInviteMapperProfile()
    {
        CreateMap<GroupInvite, GroupInviteResponse>();
    }
}