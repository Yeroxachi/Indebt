namespace Domain.Entities;

public class Group : Base
{
    public Group()
    {
        Users = new HashSet<UserGroup>();
        Invites = new HashSet<GroupInvite>();
        Merges = new HashSet<MergeRequestGroup>();
    }

    public string Name { get; set; }
    public string Description { get; set; }

    public ICollection<UserGroup> Users { get; set; }
    public ICollection<GroupInvite> Invites { get; set; }
    public ICollection<MergeRequestGroup> Merges { get; set; }
}