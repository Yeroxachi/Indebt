using Application.DTOs;
using Application.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers;

public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        CreateMap<User, UserResponse>(MemberList.Source);
        CreateMap<User, UserDto>(MemberList.Source).ReverseMap();
    }
}