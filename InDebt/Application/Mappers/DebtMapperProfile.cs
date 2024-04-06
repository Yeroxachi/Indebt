using Application.DTOs;
using Application.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers;

public class DebtMapperProfile : Profile
{
    public DebtMapperProfile()
    {
        CreateMap<DebtDto, Debt>();
        CreateMap<Debt, DebtResponse>(MemberList.Source)
            .ForMember(
                d => d.CurrencyCode,
                opt => opt.MapFrom(d => d.Currency.CurrencyCode));
    }
}