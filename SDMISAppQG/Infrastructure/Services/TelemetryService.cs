using Microsoft.AspNetCore.SignalR;
using NetTopologySuite.Geometries;
using SDMISAppQG.Database;
using SDMISAppQG.Hubs;
using SDMISAppQG.Interfaces.Hubs;
using SDMISAppQG.Models;
using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Telemetry;

namespace SDMISAppQG.Infrastructure.Services;

/// <summary>
/// Service pour traiter les données de télémétrie
/// </summary>
public class TelemetryService
{
    private readonly IHubContext<GpsHub, IGpsClient> _gpsHubContext;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<TelemetryService> _logger;

    public TelemetryService(
        IHubContext<GpsHub, IGpsClient> gpsHubContext,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<TelemetryService> logger)
    {
        _gpsHubContext = gpsHubContext;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Traite les données de télémétrie reçues
    /// </summary>
    public async Task ProcessTelemetryAsync(TelemetryData telemetryData)
    {
        if (telemetryData == null)
        {
            _logger.LogWarning("Données de télémétrie nulles reçues");
            return;
        }

        _logger.LogDebug("Données télémétrie reçues pour véhicule {TruckId}: ({Lat}, {Lng})",
            telemetryData.TruckId,
            telemetryData.Latitude,
            telemetryData.Longitude);

        // Diffusion vers le front via GpsHub
        await BroadcastPositionToFrontend(telemetryData);

        // Sauvegarde en base de données
        await SaveTelemetryToDatabase(telemetryData);
    }

    private async Task BroadcastPositionToFrontend(TelemetryData data)
    {
        await _gpsHubContext.Clients.All.ReceivePosition(
            Guid.Parse(data.TruckId),
            data.Latitude,
            data.Longitude);
    }

    private async Task SaveTelemetryToDatabase(TelemetryData data)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Recherche du véhicule
        var vehicleId = Guid.Parse(data.TruckId);
        var vehicle = await context.Vehicles.FindAsync(vehicleId);
        if (vehicle == null)
        {
            _logger.LogWarning("Véhicule avec ID {VehicleId} non trouvé en base de données.", vehicleId);
            return;
        }

        // Mise à jour de la position du véhicule
        Point point = new Point(data.Longitude, data.Latitude) { SRID = 4326 };
        vehicle.LastLocation = point;

        // Création du log de télémétrie
        var telemetryLog = new TelemetryLogsEntity
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicleId,
            Position = point,
            Timestamp = DateTime.UtcNow,
            SensorsValues = data.Levels != null
                ? data.Levels.Select(kv => SensorValue.FromTelemetry(kv.Key, kv.Value)).ToList()
                : new List<SensorValue>()
        };
        context.TelemetryLogs.Add(telemetryLog);

        await context.SaveChangesAsync();
    }
}
