namespace Domain.Entities;

public class User : Base
{
    public User()
    {
        Groups = new HashSet<UserGroup>();
        SendInvites = new HashSet<GroupInvite>();
        ReceivedInvites = new HashSet<GroupInvite>();
        Approvals = new HashSet<MergeRequestApproval>();
        Merges = new HashSet<MergeRequest>();
        SendDebts = new HashSet<Debt>();
        ReceivedDebts = new HashSet<Debt>();
        ConfirmationCodes = new HashSet<ConfirmationCode>();
        OptimizationRequestApprovals = new HashSet<OptimizationRequestApproval>();
    }

    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsConfirmedEmail { get; set; }
        
    public ICollection<UserGroup> Groups { get; set; }
    public ICollection<GroupInvite> SendInvites { get; set; }
    public ICollection<GroupInvite> ReceivedInvites { get; set; }
    public ICollection<MergeRequestApproval> Approvals { get; set; }
    public ICollection<MergeRequest> Merges { get; set; }
    public ICollection<Debt> SendDebts { get; set; }
    public ICollection<Debt> ReceivedDebts { get; set; }
    public ICollection<ConfirmationCode> ConfirmationCodes { get; set; }
    public ICollection<OptimizationRequestApproval> OptimizationRequestApprovals { get; set; }
}