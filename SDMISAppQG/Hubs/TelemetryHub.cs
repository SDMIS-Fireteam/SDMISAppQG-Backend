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

    /// <summary>
    /// Méthode appelée par la passerelle Python pour envoyer des données de télémétrie
    /// </summary>
    public async Task ReceiveTelemetry(TelemetryData telemetryData)
    {
        try
        {
            await _telemetryService.ProcessTelemetryAsync(telemetryData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du traitement des données de télémétrie");
            throw;
        }
    }
}
