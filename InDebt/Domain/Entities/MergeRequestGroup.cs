namespace Domain.Entities;

public class MergeRequestGroup
{
    public Guid Id { get; set; }
    public Guid MergeRequestId { get; set; }
    public MergeRequest MergeRequest { get; set; }
    public Guid GroupId { get; set; }
    public Group Group { get; set; }
}