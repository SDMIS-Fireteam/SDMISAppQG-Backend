namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la création d'un type d'incident
/// </summary>
public class CreateIncidentTypeDto
{
    public required string Label { get; set; }
    public required string Category { get; set; }
}
