using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(e => e.Email).IsUnique();
        builder.HasIndex(e => e.Username).IsUnique();
        builder
            .HasMany(e => e.SendInvites)
            .WithOne(e => e.Inviter)
            .HasForeignKey(e => e.InviterId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasMany(e => e.ReceivedInvites)
            .WithOne(e => e.Invited)
            .HasForeignKey(e => e.InvitedId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(e => e.SendDebts)
            .WithOne(e => e.Lender)
            .HasForeignKey(e => e.LenderId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(e => e.ReceivedDebts)
            .WithOne(e => e.Borrower)
            .HasForeignKey(e => e.BorrowerId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}