using Application.Context;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class DebtDueNotificationService : BaseService
{
    private readonly ILogger<DebtDueNotificationService> _logger;

    public DebtDueNotificationService(IMapper mapper,
        IInDebtContext context, IHttpContextAccessor accessor, ILogger<DebtDueNotificationService> logger)
        : base(context, mapper, accessor)
    {
        _logger = logger;
    }

    public async Task AddNotifiedDebts()
    {
        _logger.LogInformation($"{nameof(DebtDueNotificationService)} started");
        var debts = await Context.Debts
            .Include(d => d.Lender)
            .Where(d => d.ReminderDate != null && d.ReminderDate.Value.Date == DateTime.UtcNow.Date && d.Completed == false)
            .ToListAsync();
        var notifications = debts.Select(debt => new Notification
        {
            DebtId = debt.Id, 
            Message = $"Your debt with {debt.Lender.Name} is due on {debt.EndDate!.Value.ToShortDateString()}", 
            TimeStamp = DateTime.UtcNow
        }).ToList();

        await Context.Notifications.AddRangeAsync(notifications);
        await Context.SaveChangesAsync();
    }
}