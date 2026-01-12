using SDMISAppQG.Models.Enums;

namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la création d'un utilisateur
/// </summary>
public class CreateUserDto
{
    public Guid KeyCloakId { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Firstname { get; set; }
    public required string Lastname { get; set; }
    public required UserRole Role { get; set; }
}
