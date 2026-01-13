using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums.Vehicle;

namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO représentant un véhicule pour le front-end
/// </summary>
public class VehicleDto : BaseEntity
{
    public int IdHardware { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public VehicleAvailability Availability { get; set; }
    public VehicleUnavailabilityReason? UnavailabilityReason { get; set; }
    public float Fuel { get; set; }
    /// <summary>
    /// Consommables du véhicule stockés en JSON (ex: {"water": 1000, "foam": 200})
    /// </summary>
    public string? Consumable { get; set; }
    public int? PassengerCount { get; set; }
    public Guid TypeId { get; set; }
    public string? TypeName { get; set; }
}