namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la mise à jour d'un type d'incident
/// </summary>
public class UpdateIncidentTypeDto
{
    public string? Label { get; set; }
    public string? Category { get; set; }
}
