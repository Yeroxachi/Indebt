using Application.Context;
using Application.DTOs;
using Application.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class DebtService : BaseService, IDebtService
{
    private readonly IInDebtContext _context;
    private readonly IMapper _mapper;

    public DebtService(IInDebtContext context, IMapper mapper, IHttpContextAccessor accessor) : base(context, mapper, accessor)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse> GetAll(bool? completed, bool? isBorrower, PaginationDto paginationDto)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var query = _context.Debts.Include(d => d.Currency).AsQueryable();

        if (isBorrower.HasValue)
        {
            query = isBorrower.Value ? query.Where(x => x.BorrowerId == UserId.Value) : query.Where(x => x.LenderId == UserId.Value);
        }
            
        if (completed.HasValue)
        {
            query = query.Where(x => x.Completed == completed);
        }
            
        var receivedDebts = await query
            .Skip(paginationDto.SkipCount())
            .Take(paginationDto.PageSize)
            .ProjectTo<DebtResponse>(Mapper.ConfigurationProvider)
            .ToListAsync();
        var response = new PaginationResponse<DebtResponse>(await query.CountAsync(), receivedDebts);
        return Ok(response);
    }

    public async Task<BaseResponse> GetById(Guid debtId)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
            
        var debt = await _context.Debts
            .FirstOrDefaultAsync(d => d.Id == debtId);
        if (debt is null)
        {
            return NotFound();
        }

        if (debt.LenderId != UserId.Value && debt.BorrowerId != UserId.Value)
        {
            return Forbid();
        }

        var response = _mapper.Map<DebtResponse>(debt);
        return Ok(response);
    }

    public async Task<BaseResponse> Create(DebtDto dto)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
            
        if (UserId.Value == dto.BorrowerId)
        {
            return BadRequest<DebtResponse>("Unable to create. Lender and Borrower are same.");
        }
            
        var borrower = await _context.Users.FindAsync(dto.BorrowerId);
        var group = await _context.Groups.Include(g => g.Users)
            .FirstOrDefaultAsync(g => g.Id == dto.GroupId);
        var currency = await _context.Currencies.FirstOrDefaultAsync(c => c.CurrencyCode == dto.CurrencyCode);

        if (borrower is null || group is null || currency is null)
        {
            return NotFound();
        }

        var isUsersExitsInGroup = group.Users.Any(x => x.UserId == UserId.Value) && 
                                  group.Users.Any(x => x.UserId == borrower.Id);
        if (!isUsersExitsInGroup)
        {
            return Forbid();
        }

        var debt = new Debt
        {
            LenderId = UserId.Value,
            BorrowerId = dto.BorrowerId,
            GroupId = dto.GroupId,
            Amount = dto.Amount,
            Remainder = dto.Amount,
            Currency = currency,
            CreatedDate = DateTime.UtcNow,
            EndDate = dto.EndDate,
            ReminderDate = dto.ReminderDate
        };
        if (dto.EndDate != null && dto.EndDate.Value.Date < debt.CreatedDate.Date)
        {
            return BadRequest("Unable to create. EndDate cannot be set to a date that is earlier than the current time.");
        }

        if (dto.EndDate != null && dto.ReminderDate != null && 
            (dto.ReminderDate.Value.Date > dto.EndDate.Value.Date || dto.ReminderDate.Value.Date < debt.CreatedDate.Date))
        {
            return BadRequest("Unable to create. ReminderDate cannot be set to a date that is later than EndDate or earlier than CreateDate.");
        }

        await _context.Debts.AddAsync(debt);
        await _context.SaveChangesAsync();
        var response = _mapper.Map<DebtResponse>(debt);
        return Created(response);
    }

    public async Task<BaseResponse> Accept(Guid debtId)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
            
        var debt = await _context.Debts.FindAsync(debtId);
        if (debt == null)
        {
            return NotFound();
        }

        if (debt.BorrowerId != UserId.Value)
        {
            return Forbid();
        }

        if (debt.Approved)
        {
            return BadRequest("Debt is already approved.");
        }
            
        debt.Approved = true;
        _context.Debts.Update(debt);
        await _context.SaveChangesAsync();
        var response = _mapper.Map<DebtResponse>(debt);
        return Ok(response);
    }

    public async Task<BaseResponse> Update(Guid debtId, DebtDto dto)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
            
        var debt = await _context.Debts.FindAsync(debtId);
        if (debt is null)
        {
            return NotFound();
        }

        if (debt.LenderId != UserId.Value)
        {
            return Forbid();
        }

        if (debt.Approved)
        {
            return BadRequest("Unable to edit. Debt has already been approved.");
        }

        debt = _mapper.Map(dto, debt);
        _context.Debts.Update(debt);
        await _context.SaveChangesAsync();
        var response = _mapper.Map<DebtResponse>(debt);
        return Ok(response);
    }

    public async Task<BaseResponse> Delete(Guid debtId)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
            
        var debt = await _context.Debts.FindAsync(debtId);
        if (debt is null)
        {
            return NotFound<Debt>();
        }

        if (debt.LenderId != UserId.Value)
        {
            return Forbid();
        }

        _context.Debts.Remove(debt);
        await _context.SaveChangesAsync();
        return Ok();
    }
}