using Application.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityConfigurations;

namespace Persistence.Context;

public class InDebtContext : DbContext, IInDebtContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<GroupInvite> GroupInvites { get; set; }
    public DbSet<MergeRequest> Merges { get; set; }
    public DbSet<MergeRequestApproval> Approvals { get; set; }
    public DbSet<Debt> Debts { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<ConfirmationCode> ConfirmationCodes { get; set; }
    public DbSet<MergeRequestGroup> MergeRequestGroups { get; set; }
    public DbSet<OptimizationRequest> OptimizationRequests { get; set; }
    public DbSet<OptimizationRequestApproval> OptimizationRequestApprovals { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    protected InDebtContext()
    {
    }

    public InDebtContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(e => e.Email)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(e => e.Username)
            .IsUnique();
        
        modelBuilder
            .Entity<GroupInvite>()
            .HasOne(e=>e.Inviter)
            .WithMany(e=>e.SendInvites)
            .HasForeignKey(e=>e.InviterId)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder
            .Entity<GroupInvite>()
            .HasOne(e=>e.Invited)
            .WithMany(e=>e.ReceivedInvites)
            .HasForeignKey(e=>e.InvitedId)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder
            .Entity<MergeRequestApproval>()
            .HasOne(e => e.User)
            .WithMany(e => e.Approvals)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        
        modelBuilder.Entity<Currency>().HasIndex(c => c.CurrencyCode).IsUnique();

        modelBuilder
            .Entity<OptimizationRequestApproval>()
            .HasOne(e => e.User)
            .WithMany(e => e.OptimizationRequestApprovals)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }

    public override void Dispose()
    {
        if (Database.IsInMemory())
        {
            Database.EnsureDeleted();
        }
        base.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        if (Database.IsInMemory())
        {
            await Database.EnsureDeletedAsync();
        }
        await base.DisposeAsync();
    }
}