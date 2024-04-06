using Application.Services;
using Hangfire;

namespace InDebt.Jobs;

public class RecurringJobs
{
    public static void Register()
    {
        RecurringJob.AddOrUpdate<DebtDueNotificationService>(nameof(DebtDueNotificationService),
            job => job.AddNotifiedDebts(), Cron.Daily, TimeZoneInfo.Utc);
    }
}