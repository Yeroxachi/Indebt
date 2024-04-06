using Application.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers;

public class CurrencyMapperProfile : Profile
{
    public CurrencyMapperProfile()
    {
        CreateMap<Currency, CurrencyResponse>();
    }
}