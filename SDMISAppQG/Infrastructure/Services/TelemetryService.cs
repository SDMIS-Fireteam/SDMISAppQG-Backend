using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
public class TelemetryService {
   private readonly IHubContext<GpsHub, IGpsClient> _gpsHubContext;
   private readonly IServiceScopeFactory _serviceScopeFactory;
   private readonly ILogger<TelemetryService> _logger;

   public TelemetryService(
       IHubContext<GpsHub, IGpsClient> gpsHubContext,
       IServiceScopeFactory serviceScopeFactory,
       ILogger<TelemetryService> logger) {
      _gpsHubContext = gpsHubContext;
      _serviceScopeFactory = serviceScopeFactory;
      _logger = logger;
   }

   /// <summary>
   /// Traite les données de télémétrie reçues
   /// </summary>
   public async Task ProcessTelemetryAsync(TelemetryData telemetryData) {
      if (telemetryData is null) {
         _logger.LogWarning("Données de télémétrie nulles reçues");
         return;
      }

      _logger.LogDebug("Données télémétrie reçues pour véhicule IdHardware={IdHardware}: ({Lat}, {Lng})",
          telemetryData.IdHardware,
          telemetryData.Latitude,
          telemetryData.Longitude);

      // Diffusion vers le front via GpsHub
      await BroadcastPositionToFrontend(telemetryData);

      // Sauvegarde en base de données
      await SaveTelemetryToDatabase(telemetryData);
   }

   private async Task BroadcastPositionToFrontend(TelemetryData data) {
      // Récupération du véhicule pour obtenir son GUID
      using var scope = _serviceScopeFactory.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

      VehicleEntity? vehicle = await context.Vehicles.FirstOrDefaultAsync(v => v.IdHardware == data.IdHardware);
      if (vehicle is null) {
         _logger.LogWarning("Impossible de diffuser la position: véhicule IdHardware={IdHardware} non trouvé", data.IdHardware);
         return;
      }

      Point point = new Point(data.Longitude, data.Latitude) { SRID = 4326 };
      vehicle.LastLocation = point;

      await _gpsHubContext.Clients.All.ReceivePosition(vehicle);
   }

   private async Task SaveTelemetryToDatabase(TelemetryData data) {
      using var scope = _serviceScopeFactory.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

      // Recherche du véhicule par IdHardware
      VehicleEntity? vehicle = await context.Vehicles.FirstOrDefaultAsync(v => v.IdHardware == data.IdHardware);
      if (vehicle is null) {
         _logger.LogWarning("Véhicule avec IdHardware={IdHardware} non trouvé en base de données.", data.IdHardware);
         return;
      }

      // Mise à jour de la position du véhicule
      Point point = new Point(data.Longitude, data.Latitude) { SRID = 4326 };
      vehicle.LastLocation = point;
      vehicle.Fuel = (float?)data.Levels?["fuel"] ?? vehicle.Fuel;
      vehicle.Consumable = data.Levels != null
          ? System.Text.Json.JsonSerializer.Serialize(data.Levels.Where(kv => kv.Key != "fuel").ToDictionary(kv => kv.Key, kv => kv.Value))
          : vehicle.Consumable;

      // Création du log de télémétrie
      var telemetryLog = new TelemetryLogsEntity {
         Id = Guid.NewGuid(),
         CreatedAt = DateTime.UtcNow,
         VehicleId = vehicle.Id,
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
