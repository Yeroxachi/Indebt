using Application.Context;
using Application.Responses;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class RatingService : BaseService, IRatingService
{
    public RatingService(IInDebtContext context, IMapper mapper, IHttpContextAccessor accessor) : base(context, mapper, accessor)
    {
    }

    public async Task<BaseResponse> GetRatingOfUserByIdAsync(Guid userId)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var user = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
        {
            return NotFound();
        }
        
        var debts = await Context.Debts.Where(x => x.BorrowerId == userId).ToListAsync();
        var response = CalculateRating(userId, debts);

        return Ok(response);
    }

    public async Task<BaseResponse> GetRatingOfUsersByGroupIdAsync(Guid groupId)
    {
        if (UserId is null || UserGroups is null)
        {
            return UnAuthorize();
        }

        if (!UserGroups.Contains(groupId))
        {
            return Forbid();
        }

        var group = await Context.Groups
            .Include(x => x.Users)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == groupId);
        if (group is null)
        {
            return NotFound();
        }

        var listOfUsers = group.Users.Select(x => x.UserId).ToList();
        var debts = await Context.Debts.Where(x => x.GroupId == groupId).AsNoTracking().ToListAsync();
        var responses = listOfUsers.Select(user => CalculateRating(user, debts)).ToList();

        return Ok(responses);
    }

    private static RatingResponse CalculateRating(Guid userId, ICollection<Debt> debts)
    {
        var paidDebtsCount = debts.Count(x => x.BorrowerId == userId && x.Completed);
        var debtsCount = debts.Count(x => x.BorrowerId == userId);
        var rating = 100.00;
        if (debtsCount != 0)
        {
            rating = (double)paidDebtsCount / debtsCount * 100;
        }

        return new RatingResponse
        {
            UserId = userId,
            Rating = rating
        };
    }
}