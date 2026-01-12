using Microsoft.EntityFrameworkCore;

using SDMISAppQG.Models.Enums;

namespace SDMISAppQG.Models.Entities;

[Index(nameof(Email), IsUnique = true)]
public class UserEntity : BaseEntity {
   public Guid KeyCloakId { get; set; }
   public required string Email { get; set; }
   public required string Username { get; set; }
   public required string Firstname { get; set; }
   public required string Lastname { get; set; }
   public required UserRole Role { get; set; }
   public bool IsDeleted { get; set; } = false;
}
