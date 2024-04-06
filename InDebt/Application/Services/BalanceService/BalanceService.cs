using Application.Context;
using Application.DTOs;
using Application.Responses;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class BalanceService : BaseService, IBalanceService
{
    private readonly IExchangeRateService _exchangeRateService;

    public BalanceService(IInDebtContext context, IMapper mapper, IHttpContextAccessor accessor, IExchangeRateService exchangeRateService) : base(context, mapper, accessor)
    {
        _exchangeRateService = exchangeRateService;
    }

    public async Task<BaseResponse> GetTotal(Guid? groupId)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var query = Context.Debts
            .Where(x => (x.LenderId == UserId.Value || x.BorrowerId == UserId.Value) && !x.Completed && x.Approved);
        if (groupId.HasValue)
        {
            query = query.Where(x => x.GroupId == groupId.Value);
        }

        var debts = await query.ToListAsync();
        var currency = await PriorityCurrencyCode(UserId.Value);
        var lendedDebtsSum = new decimal();
        var borrowedDebtsSum = new decimal();
        foreach (var debt in debts)
        {
            var amount = new decimal();
            if (debt.CurrencyId != currency.Id)
            {
                var newDto = new ExchangeRateDto
                {
                    Amount = debt.Remainder,
                    LeftCurrencyId = debt.CurrencyId,
                    RightCurrencyId = currency.Id
                };
                var resultOfExchange = await _exchangeRateService.CalculateExchangeRateAsync(newDto);
                amount = resultOfExchange.Data.ConversionResult;
            }
            else
            {
                amount = debt.Remainder;
            }
            
            if (debt.LenderId == UserId.Value)
            {
                lendedDebtsSum += amount;
            }
            else
            {
                borrowedDebtsSum += amount;
            }
        }

        var total = lendedDebtsSum - borrowedDebtsSum;
        var response = new BalanceResponse
        {
            Type = BalanceType.Total,
            Balance = total,
            CurrencyCode = currency.CurrencyCode,
            GroupId = groupId.ToString()
        };
        return Ok(response);
    }

    public async Task<BaseResponse> GetTotalOutcome(Guid? groupId)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
        
        var query = Context.Debts
            .Where(x => x.BorrowerId == UserId.Value && !x.Completed && x.Approved);
        if (groupId.HasValue)
        {
            query = query.Where(x => x.GroupId == groupId.Value);
        }
        
        var debts = await query.ToListAsync();
        var currency = await PriorityCurrencyCode(UserId.Value);
        var borrowedDebtsSum = new decimal();
        foreach (var debt in debts)
        {
            var amount = new decimal();
            if (debt.CurrencyId != currency.Id)
            {
                var newDto = new ExchangeRateDto
                {
                    Amount = debt.Remainder,
                    LeftCurrencyId = debt.CurrencyId,
                    RightCurrencyId = currency.Id
                };
                var resultOfExchange = await _exchangeRateService.CalculateExchangeRateAsync(newDto);
                amount = resultOfExchange.Data.ConversionResult;
            }
            else
            {
                amount = debt.Remainder;
            }
            
            if (debt.BorrowerId == UserId.Value)
            {
                borrowedDebtsSum += amount;
            }
        }

        var response = new BalanceResponse
        {
            Type = BalanceType.Incoming,
            Balance = borrowedDebtsSum,
            CurrencyCode = currency.CurrencyCode,
            GroupId = groupId.ToString()
        };
        return Ok(response);
    }

    public async Task<BaseResponse> GetTotalIncome(Guid? groupId)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
        
        var query = Context.Debts
            .Where(x => x.LenderId == UserId.Value && !x.Completed && x.Approved);
        if (groupId.HasValue)
        {
            query = query.Where(x => x.GroupId == groupId.Value);
        }

        var debts = await query.ToListAsync();
        var currency = await PriorityCurrencyCode(UserId.Value);
        var lendedDebtsSum = new decimal();
        foreach (var debt in debts)
        {
            var amount = new decimal();
            if (debt.CurrencyId != currency.Id)
            {
                var newDto = new ExchangeRateDto
                {
                    Amount = debt.Remainder,
                    LeftCurrencyId = debt.CurrencyId,
                    RightCurrencyId = currency.Id
                };
                var resultOfExchange = await _exchangeRateService.CalculateExchangeRateAsync(newDto);
                amount = resultOfExchange.Data.ConversionResult;
            }
            else
            {
                amount = debt.Remainder;
            }
            
            if (debt.LenderId == UserId.Value)
            {
                lendedDebtsSum += amount;
            }
        }

        var response = new BalanceResponse
        {
            Type = BalanceType.Outgoing,
            Balance = lendedDebtsSum,
            CurrencyCode = currency.CurrencyCode,
            GroupId = groupId.ToString()
        };
        return Ok(response);
    }

    private async Task<Currency> PriorityCurrencyCode(Guid userId)
    {
        var priorityCurrency = DefaultCurrencyId!.Value;
        var isHadSomeDebts = await Context.Debts.AnyAsync(x => x.LenderId == userId);
        if (isHadSomeDebts)
        {
            var debtsOfUser = await Context.Debts.Where(x => x.LenderId == userId)
                .ToListAsync();
            priorityCurrency = debtsOfUser
                .GroupBy(y => y.CurrencyId)
                .OrderByDescending(x => x.Count())
                .First()
                .Key;
        }
        var currency = await Context.Currencies.FirstOrDefaultAsync(x => x.Id == priorityCurrency);
        return currency;
    }
}