using System.Text.Json.Serialization;

namespace SDMISAppQG.Models.Telemetry;

/// <summary>
/// Représente les données de télémétrie reçues de la passerelle Python
/// </summary>
public class TelemetryData
{
    [JsonPropertyName("truckId")]
    public string TruckId { get; set; } = string.Empty;
    
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }
    
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
    
    [JsonPropertyName("levels")]
    public Dictionary<string, double>? Levels { get; set; }
}
