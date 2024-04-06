using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Context;

public interface IInDebtContext
{
    DbSet<User> Users { get; set; }
    DbSet<Group> Groups { get; set; }
    DbSet<UserGroup> UserGroups { get; set; }
    DbSet<GroupInvite> GroupInvites { get; set; }
    DbSet<MergeRequest> Merges { get; set; }
    DbSet<MergeRequestGroup> MergeRequestGroups { get; set; }
    DbSet<MergeRequestApproval> Approvals { get; set; }
    DbSet<Debt> Debts { get; set; }
    DbSet<Currency> Currencies { get; set; }
    DbSet<Transaction> Transactions { get; set; }
    DbSet<ConfirmationCode> ConfirmationCodes { get; set; }
    DbSet<OptimizationRequest> OptimizationRequests { get; set; }
    DbSet<OptimizationRequestApproval> OptimizationRequestApprovals { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }
    DbSet<Notification> Notifications { get; set; }
    Task<int> SaveChangesAsync();
}