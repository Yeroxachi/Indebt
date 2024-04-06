using Application.Context;
using Application.DTOs;
using Application.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class NotificationService : BaseService, INotificationService
{
    public NotificationService(IInDebtContext context, IMapper mapper, IHttpContextAccessor accessor) 
        : base(context, mapper, accessor)
    {
            
    }

    public async Task<BaseResponse> GetAllUnreadAsync(PaginationDto dto)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var totalCount = await Context.Notifications
            .Where(n => n.Debt.BorrowerId == UserId && n.IsRead == false)
            .CountAsync();
        var notifications = await Context.Notifications
            .Include(n => n.Debt)
            .Where(n => n.Debt.BorrowerId == UserId && n.IsRead == false)
            .Skip(dto.SkipCount())
            .Take(dto.PageSize)
            .AsNoTracking()
            .ProjectTo<NotificationResponse>(Mapper.ConfigurationProvider)
            .ToListAsync();
        var response = new PaginationResponse<NotificationResponse>(totalCount, notifications);
        return Ok(response);
    }

    public async Task<BaseResponse> CreateAsync(NotificationDto dto)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var notification = Mapper.Map<Notification>(dto);
        await Context.Notifications.AddAsync(notification);
        await Context.SaveChangesAsync();
        return Created();
    }

    public async Task<BaseResponse> MarkAsReadAsync(Guid id)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var notification = await Context.Notifications
            .Include(n => n.Debt)
            .FirstOrDefaultAsync(n => n.Id == id);
        if (notification is null)
        {
            return NotFound();
        }

        if (notification.IsRead)
        {
            return BadRequest("Notification is already read");
        }

        if (notification.Debt.BorrowerId != UserId)
        {
            return Forbid();
        }

        notification.IsRead = true;
        await Context.SaveChangesAsync();
        return Ok(Mapper.Map<NotificationResponse>(notification));
    }

    public async Task<BaseResponse> DeleteAsync(Guid id)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var notification = await Context.Notifications
            .Include(n => n.Debt)
            .FirstOrDefaultAsync(n => n.Id == id);
        if (notification is null)
        {
            return NotFound();
        }

        if (notification.Debt.BorrowerId != UserId)
        {
            return Forbid();
        }

        Context.Notifications.Remove(notification);
        await Context.SaveChangesAsync();
        return Ok();
    }
}