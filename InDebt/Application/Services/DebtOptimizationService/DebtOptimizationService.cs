using Application.Context;
using Application.DTOs;
using Application.Responses;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class DebtOptimizationService : BaseService , IDebtOptimizationService
{
    private readonly IExchangeRateService _exchangeRateService;

    public DebtOptimizationService(IInDebtContext context, IMapper mapper, IHttpContextAccessor accessor, IExchangeRateService exchangeRateService) : base(context, mapper, accessor)
    {
        _exchangeRateService = exchangeRateService;
    }

    public async Task<BaseResponse> GetAllReceivedApprovalsAsync()
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var receivedApprovals = await Context.OptimizationRequestApprovals
            .Where(x => x.UserId == UserId.Value)
            .ToListAsync();

        var response = Mapper.Map<IReadOnlyCollection<OptimizationApprovalResponse>>(receivedApprovals);
        return Ok(response);
    }

    public async Task<BaseResponse> GetAllAsync()
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var requests = await Context.OptimizationRequests
            .Where(x => x.Group.Users.Any(y => y.UserId == UserId.Value))
            .ToListAsync();

        var response = Mapper.Map<IReadOnlyCollection<OptimizationResponse>>(requests);
        return Ok(response);
    }

    public async Task<BaseResponse> GetByIdAsync(Guid id)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var request = await Context.OptimizationRequests
            .FirstOrDefaultAsync(x => x.Group.Users.Any(y => y.UserId == UserId.Value && x.Id == id));

        if (request is null)
        {
            return NotFound();
        }
        
        var response = Mapper.Map<OptimizationResponse>(request);
        return Ok(response);
    }

    public async Task<BaseResponse> CreateAsync(Guid groupId)
    {
        if (UserId is null || UserGroups is null)
        {
            return UnAuthorize();
        }

        if (!UserGroups.Contains(groupId))
        {
            return Forbid();
        }

        var users = await Context.UserGroups
            .Where(x => x.GroupId == groupId && x.UserId != UserId.Value)
            .ToListAsync();

        var newOptimizationRequest = new OptimizationRequest
        {
            GroupId = groupId,
            InitiatorId = UserId.Value,
            Approvals = users.Select(x => new OptimizationRequestApproval
            {
                UserId = x.UserId
            }).ToList()
        };

        await Context.OptimizationRequests.AddAsync(newOptimizationRequest);
        await Context.SaveChangesAsync();
        var response = Mapper.Map<OptimizationResponse>(newOptimizationRequest);
        return Created(response);
    }

    public async Task<BaseResponse> AcceptApprovalAsync(Guid id)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var approval = await Context.OptimizationRequestApprovals.FirstOrDefaultAsync(x => x.Id == id);
        if (approval is null)
        {
            return NotFound();
        }

        if (approval.UserId != UserId.Value)
        {
            return Forbid();
        }

        if (approval.Approved)
        {
            return BadRequest("Optimization request is already approved");
        }

        approval.Approved = true;
        await Context.SaveChangesAsync();
        var response = Mapper.Map<OptimizationApprovalResponse>(approval);
        return Ok(response);
    }

    public async Task<BaseResponse> OptimizeAsync(Guid id)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        if (DefaultCurrencyId is null)
        {
            return BadRequest("Service not working. Try later");
        }
        
        var optimization = await Context.OptimizationRequests
            .Include(x => x.Approvals)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (optimization is null)
        {
            return NotFound();
        }

        if (optimization.InitiatorId != UserId.Value)
        {
            return Forbid();
        }

        if (optimization.Status != OptimizationStatus.Pending)
        {
            return BadRequest($"Optimization request has status {optimization.Status.ToString()}");
        }

        var users = optimization.Approvals
            .Where(x => x.Approved).Select(x => x.UserId).ToList();
        users.Add(UserId.Value);

        var debts = await Context.Debts
            .Include(x => x.Currency)
            .Where(x => users.Any(y => y == x.LenderId) && users.Any(y => y == x.BorrowerId) && !x.Completed)
            .ToListAsync();

        var balance = BalancesForUsers(debts, users, DefaultCurrencyId.Value);
        var debtsForOptimization = DistributeBalances(balance, optimization.GroupId);
        await CreateNewDebts(debtsForOptimization, DefaultCurrencyId.Value);
        if(debtsForOptimization.Count > 0)
        {
            CompleteDebt(debts);
        }
        
        optimization.Status = OptimizationStatus.Optimized;
        await Context.SaveChangesAsync();
        return Ok();
    }

    private  List<UserBalanceModel> BalancesForUsers(IReadOnlyCollection<Debt> debts, IEnumerable<Guid> users, Guid defaultCurrencyId)
    {
        var balances = new List<UserBalanceModel>();
        foreach (var user in users)
        {
            var specifiedDebts = debts.Where(x => x.LenderId == user || x.BorrowerId == user)
                .Select(x => new
                {
                    x.LenderId,
                    x.BorrowerId,
                    x.CurrencyId,
                    Remainder = _exchangeRateService.CalculateExchangeRateAsync(new ExchangeRateDto
                    {
                        LeftCurrencyId = x.CurrencyId,
                        RightCurrencyId = defaultCurrencyId,
                        Amount = x.Remainder
                    }).Result.Data.ConversionResult
                }).ToList();
            var overall = specifiedDebts.Where(x => x.LenderId == user).Sum(x => x.Remainder) -
                          specifiedDebts.Where(x => x.BorrowerId == user).Sum(x => x.Remainder);
            
            var priorityCurrency = DefaultCurrencyId!.Value;
            if (debts.FirstOrDefault(x => x.LenderId == user) is not null)
            {
                priorityCurrency = debts.Where(x => x.LenderId == user)
                    .GroupBy(y => y.CurrencyId)
                    .OrderByDescending(x => x.Count())
                    .First()
                    .Key;
            }
            
            balances.Add(new UserBalanceModel
            {
                UserId = user,
                Balance = overall,
                PriorityCurrencyId = priorityCurrency
            });
        }
        return balances.Where(x => x.Balance != 0).ToList();
    }

    private static List<DebtOptimizationModel> DistributeBalances(IList<UserBalanceModel> balance, Guid groupId)
    {
        var newDebtsList = new List<DebtOptimizationModel>();
        var leftSide = 0;
        var rightSide = balance.Count - 1;
        while (leftSide <= rightSide)
        {
            var borrowerBalance = Math.Abs(balance[leftSide].Balance);
            var lenderBalance = Math.Abs(balance[rightSide].Balance);
            if (borrowerBalance > lenderBalance)
            {
                var newBalance = borrowerBalance - lenderBalance;
                balance[leftSide].Balance = newBalance;
                var newDebt = new DebtOptimizationModel
                {
                    LenderId = balance[rightSide].UserId, 
                    BorrowerId = balance[leftSide].UserId, 
                    GroupId = groupId, 
                    Amount = lenderBalance,
                    CurrencyId = balance[rightSide].PriorityCurrencyId
                };
                newDebtsList.Add(newDebt);
                rightSide--;
            }
            else if (borrowerBalance < lenderBalance)
            {
                var newBalance = lenderBalance - borrowerBalance;
                balance[rightSide].Balance = newBalance;
                var newDebt = new DebtOptimizationModel
                {
                    LenderId = balance[rightSide].UserId, 
                    BorrowerId = balance[leftSide].UserId, 
                    GroupId = groupId, 
                    Amount = borrowerBalance,
                    CurrencyId = balance[rightSide].PriorityCurrencyId
                };
                newDebtsList.Add(newDebt);
                leftSide++;
            }
            else
            {
                var newDebt = new DebtOptimizationModel
                {
                    LenderId = balance[rightSide].UserId, 
                    BorrowerId = balance[leftSide].UserId, 
                    GroupId = groupId, 
                    Amount = borrowerBalance,
                    CurrencyId = balance[rightSide].PriorityCurrencyId
                };
                newDebtsList.Add(newDebt);
                leftSide++;
                rightSide--;
            }
        }
        return newDebtsList;
    }

    private async Task CreateNewDebts(IEnumerable<DebtOptimizationModel> newDebts, Guid defaultCurrencyId)
    {
        var debts = new List<Debt>();
        
        foreach (var newDebt in newDebts)
        {
            var amount = await _exchangeRateService.CalculateExchangeRateAsync(new ExchangeRateDto
            {
                LeftCurrencyId = defaultCurrencyId,
                RightCurrencyId = newDebt.CurrencyId,
                Amount = newDebt.Amount
            });
            debts.Add(new Debt
            {
                GroupId = newDebt.GroupId,
                LenderId = newDebt.LenderId,
                BorrowerId = newDebt.BorrowerId,
                Amount = amount.Data.ConversionResult,
                Remainder = amount.Data.ConversionResult,
                Approved = true,
                CurrencyId = newDebt.CurrencyId,
                CreatedDate = DateTime.Now,
            });
        }

        await Context.Debts.AddRangeAsync(debts);
    }

    private static void CompleteDebt(List<Debt> debts)
    {
        foreach (var debt in debts)
        {
            debt.Remainder = 0;
            debt.Completed = true;
        }
    }
}