using Application.DTOs;
using Application.Responses;
using AutoMapper;
using Group = Domain.Entities.Group;

namespace Application.Mappers;

public class GroupMapperProfile : Profile
{
    public GroupMapperProfile()
    {
        CreateMap<Group, GroupResponse>(MemberList.Source).ReverseMap();
        CreateMap<Group, GroupDto>(MemberList.Source).ReverseMap();
    }
}