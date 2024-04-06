using Application.Context;
using Application.DTOs;
using Application.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class TransactionService : BaseService, ITransactionService
{
    private readonly IInDebtContext _context;
    private readonly IMapper _mapper;
    private readonly IExchangeRateService _exchangeRate;

    public TransactionService(IInDebtContext context, IMapper mapper, IHttpContextAccessor accessor, IExchangeRateService exchangeRate) : base(context, mapper, accessor)
    {
        _context = context;
        _mapper = mapper;
        _exchangeRate = exchangeRate;
    }

    public async Task<BaseResponse> GetAll(Guid? debtId, TransactionType? transactionType, PaginationDto paginationDto)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var query = Context.Transactions
            .Include(t => t.Debt)
            .ThenInclude(d => d.Currency)
            .AsQueryable();

        if (debtId.HasValue)
        {
            query = query.Where(x => x.Debt.Id == debtId.Value);
        }
            
        if (transactionType.HasValue)
        {
            query = transactionType.Value == TransactionType.Incoming 
                ? query.Where(x => x.Debt.LenderId == UserId.Value) 
                : query.Where(x => x.Debt.BorrowerId == UserId.Value);
        }

        var transactions = await query
            .Skip(paginationDto.SkipCount())
            .Take(paginationDto.PageSize)
            .ProjectTo<TransactionResponse>(Mapper.ConfigurationProvider)
            .ToListAsync();
            
        var response = new PaginationResponse<TransactionResponse>(await query.CountAsync(), transactions);

        foreach (var transaction in response.Data)
        {
            transaction.SetTransactionType(UserId.Value);
        }

        return Ok(response);
    }

    public async Task<BaseResponse> GetById(Guid id)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var transactions = await _context.Transactions
            .Include(t => t.Debt)
            .ThenInclude(d => d.Currency)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transactions is null)
        {
            return NotFound();
        }
            
        var isUserNotOwner = transactions.Debt.LenderId != UserId.Value &&
                             transactions.Debt.BorrowerId != UserId.Value;
        if (isUserNotOwner)
        {
            return Forbid();
        }
            
        var response = Mapper.Map<TransactionResponse>(transactions);
        response.SetTransactionType(UserId.Value);
        return Ok(response);
    }
        

    public async Task<BaseResponse> Create(TransactionDto dto)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
            
        var debt = await Context.Debts.Include(d => d.Currency).FirstOrDefaultAsync(x => x.Id == dto.DebtId && x.Approved);
        var currency = await Context.Currencies.FirstOrDefaultAsync(c => c.CurrencyCode == dto.CurrencyCode);
        if (debt is null || currency is null)
        {
            return NotFound();
        }
            
        if (debt.BorrowerId != UserId.Value)
        {
            return Forbid();    
        }

        if (debt.Completed)
        {
            return BadRequest("Debt is already completed");
        }

        if (currency.CurrencyCode != debt.Currency.CurrencyCode)
        {
            var exchangeRateDto = new ExchangeRateDto()
            {
                LeftCurrencyId = currency.Id,
                RightCurrencyId = debt.Currency.Id,
                Amount = dto.Amount
            };
            var exchangeResponse = await _exchangeRate.CalculateExchangeRateAsync(exchangeRateDto);
            dto.Amount = exchangeResponse.Data.ConversionResult;
        }
            
        if (dto.Amount > debt.Remainder)
        {
            return BadRequest("Provided amount is higher than remained amount.");
        }

        var transaction = new Transaction
        {
            Amount = dto.Amount,
            DebtId = debt.Id
        };
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
        var response = _mapper.Map<TransactionResponse>(transaction);
        response.SetTransactionType(UserId.Value);
        return Created(response);
    }

    public async Task<BaseResponse> Accept(Guid transactionId)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
            
        var transaction = await _context.Transactions
            .Include(t => t.Debt)
            .FirstOrDefaultAsync(t => t.Id == transactionId);
        if (transaction is null)
        {
            return NotFound();
        }

        if (transaction.Debt.LenderId != UserId.Value)
        {
            return Forbid();
        }

        if (transaction.Approved)
        {
            return BadRequest("Transaction is already approved.");
        }

        if (transaction.Debt.Remainder < transaction.Amount)
        {
            return BadRequest("Amount is more than Remainder");
        }
            
        transaction.Approved = true;
        transaction.Debt.Remainder -= transaction.Amount;
        if (transaction.Debt.Remainder == 0)
        {
            transaction.Debt.Completed = true;
        }

        await _context.SaveChangesAsync();
        var response = _mapper.Map<TransactionResponse>(transaction);
        response.SetTransactionType(UserId.Value);
        return Ok(response);
    }

    public async Task<BaseResponse> Update(Guid transactionId, TransactionDto dto)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
            
        var transaction = await _context.Transactions.Include(x => x.Debt).FirstOrDefaultAsync(t => t.Id == transactionId);
        if (transaction is null)
        {
            return NotFound();
        }

        if (transaction.Debt.BorrowerId != UserId.Value)
        {
            return Forbid();
        }

        if (transaction.Approved)
        {
            return BadRequest("Unable to edit. Transaction has already been approved.");
        }

        transaction = _mapper.Map(dto, transaction);
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        var response = _mapper.Map<TransactionResponse>(transaction);
        response.SetTransactionType(UserId.Value);
        return Ok(response);
    }

    public async Task<BaseResponse> Delete(Guid transactionId)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
            
        var transaction = await _context.Transactions.FindAsync(transactionId);
        if (transaction is null)
        {
            return NotFound();
        }

        if (transaction.Debt.LenderId != UserId.Value)
        {
            return Forbid();
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return Ok();
    }
}