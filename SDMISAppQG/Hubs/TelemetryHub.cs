using Microsoft.AspNetCore.SignalR;
using SDMISAppQG.Infrastructure.Services;
using SDMISAppQG.Models.Telemetry;

namespace SDMISAppQG.Hubs;

/// <summary>
/// Hub SignalR pour recevoir les données de télémétrie depuis la passerelle Python
/// </summary>
public class TelemetryHub : Hub
{
    private readonly TelemetryService _telemetryService;
    private readonly ILogger<TelemetryHub> _logger;

    public TelemetryHub(
        TelemetryService telemetryService,
        ILogger<TelemetryHub> logger)
    {
        _telemetryService = telemetryService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client Python connecté: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client Python déconnecté: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Méthode appelée par la passerelle Python pour envoyer des données de télémétrie
    /// Accepte les paramètres individuellement pour compatibilité avec le client Python
    /// </summary>
    public async Task ReceiveTelemetry(string data)
    {
        var telemetryData = System.Text.Json.JsonSerializer.Deserialize<TelemetryData>(data);

        _logger.LogInformation("ReceiveTelemetry appelé - ConnectionId: {ConnectionId}", Context.ConnectionId);
        try
        {

            _logger.LogInformation("ReceiveTelemetry - Données reçues: IdHardware={IdHardware}, Lat={Lat}, Lng={Lng}, Levels={@Levels}",
                telemetryData.IdHardware,
                telemetryData.Latitude,
                telemetryData.Longitude,
                telemetryData.Levels);

            await _telemetryService.ProcessTelemetryAsync(telemetryData);
            _logger.LogInformation("Télémétrie traitée avec succès pour le camion IdHardware={IdHardware}", telemetryData.IdHardware);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du traitement des données de télémétrie pour le camion IdHardware={IdHardware}", telemetryData?.IdHardware);
            throw;
        }
    }
}
