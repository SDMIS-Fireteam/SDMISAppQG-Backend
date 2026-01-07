namespace SDMISAppQG.Models.Entities;

public class UserEntity : BaseEntity
{
    public Guid KeyCloakId { get; set; }
    public required string Username { get; set; }
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
    public bool IsDeleted { get; set; } = false;
}
