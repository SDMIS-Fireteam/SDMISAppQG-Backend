using NetTopologySuite.Geometries;

namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la création d'un log de télémétrie
/// </summary>
public class CreateTelemetryLogDto
{
    public required Guid VehicleId { get; set; }
    public required Point Position { get; set; }
    public required DateTime Timestamp { get; set; }
    public required List<SensorValue> SensorsValues { get; set; }
}
