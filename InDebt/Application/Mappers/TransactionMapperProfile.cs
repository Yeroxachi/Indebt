using Application.DTOs;
using Application.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers;

public class TransactionMapperProfile : Profile
{
    public TransactionMapperProfile()
    {
        CreateMap<TransactionDto, Transaction>();
        CreateMap<Transaction, TransactionResponse>(MemberList.Source)
            .ForMember(x => x.LenderId, opt => opt.MapFrom(t => t.Debt.LenderId))
            .ForMember(x => x.BorrowerId, opt => opt.MapFrom(t => t.Debt.BorrowerId))
            .ForMember(x => x.Currency, opt => opt.MapFrom(t => t.Debt.Currency.CurrencyCode));
    }
}