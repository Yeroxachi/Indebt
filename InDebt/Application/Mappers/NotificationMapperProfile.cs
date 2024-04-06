using Application.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers;

public class NotificationMapperProfile : Profile
{
    public NotificationMapperProfile()
    {
        CreateMap<Notification, NotificationResponse>().ReverseMap();
    }
}