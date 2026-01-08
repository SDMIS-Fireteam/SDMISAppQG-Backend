using NetTopologySuite.Geometries;

namespace SDMISAppQG.Models.DTOs;

/// <summary>
/// DTO pour la mise à jour d'un log de télémétrie
/// </summary>
public class UpdateTelemetryLogDto
{
    public Guid? VehicleId { get; set; }
    public Point? Position { get; set; }
    public DateTime? Timestamp { get; set; }
    public List<SensorValue>? SensorsValues { get; set; }
}
