using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SDMISAppQG.Models.Enums.Vehicle;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDMISAppQG.Models.Entities;

[Index(nameof(IdHardware), IsUnique = true)]
public class VehicleEntity : BaseEntity
{
    /// <summary>
    /// Identifiant matériel unique du véhicule (du micro:bit ou dispositif embarqué)
    /// </summary>
    public required int IdHardware { get; set; }
    public required Guid TypeId { get; set; }
    [Column(TypeName = "geography(Point, 4326)")]
    public Point? LastLocation { get; set; }
    public required VehicleAvailability Availability { get; set; }
    public required VehicleUnavailabilityReason UnavailabilityReason { get; set; }
    public required float Fuel { get; set; }
}
