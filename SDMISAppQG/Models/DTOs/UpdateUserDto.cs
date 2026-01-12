namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la mise à jour d'un utilisateur
/// </summary>
public class UpdateUserDto
{
    public Guid? KeyCloakId { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public bool? IsDeleted { get; set; }
}
