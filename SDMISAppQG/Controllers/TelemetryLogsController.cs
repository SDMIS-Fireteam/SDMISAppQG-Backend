using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMISAppQG.Database;
using SDMISAppQG.Models.DTOs;
using SDMISAppQG.Models.Entities;

namespace SDMISAppQG.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TelemetryLogsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TelemetryLogsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère tous les logs de télémétrie
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TelemetryLogsEntity>>> GetTelemetryLogs()
    {
        return await _context.TelemetryLogs.ToListAsync();
    }

    /// <summary>
    /// Récupère un log de télémétrie par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TelemetryLogsEntity>> GetTelemetryLog(Guid id)
    {
        var telemetryLog = await _context.TelemetryLogs.FindAsync(id);

        if (telemetryLog == null)
        {
            return NotFound();
        }

        return telemetryLog;
    }

    /// <summary>
    /// Récupère tous les logs de télémétrie pour un véhicule spécifique
    /// </summary>
    [HttpGet("vehicle/{vehicleId}")]
    public async Task<ActionResult<IEnumerable<TelemetryLogsEntity>>> GetTelemetryLogsByVehicle(Guid vehicleId)
    {
        return await _context.TelemetryLogs
            .Where(t => t.VehicleId == vehicleId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();
    }

    /// <summary>
    /// Crée un nouveau log de télémétrie
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TelemetryLogsEntity>> CreateTelemetryLog(CreateTelemetryLogDto dto)
    {
        var telemetryLog = new TelemetryLogsEntity
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            VehicleId = dto.VehicleId,
            Position = dto.Position,
            Timestamp = dto.Timestamp,
            SensorsValues = dto.SensorsValues
        };

        _context.TelemetryLogs.Add(telemetryLog);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTelemetryLog), new { id = telemetryLog.Id }, telemetryLog);
    }

    /// <summary>
    /// Met à jour un log de télémétrie existant
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTelemetryLog(Guid id, UpdateTelemetryLogDto dto)
    {
        var telemetryLog = await _context.TelemetryLogs.FindAsync(id);
        if (telemetryLog == null)
        {
            return NotFound();
        }

        if (dto.VehicleId.HasValue)
            telemetryLog.VehicleId = dto.VehicleId.Value;
        if (dto.Position != null)
            telemetryLog.Position = dto.Position;
        if (dto.Timestamp.HasValue)
            telemetryLog.Timestamp = dto.Timestamp.Value;
        if (dto.SensorsValues != null)
            telemetryLog.SensorsValues = dto.SensorsValues;

        telemetryLog.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await TelemetryLogExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Supprime un log de télémétrie
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTelemetryLog(Guid id)
    {
        var telemetryLog = await _context.TelemetryLogs.FindAsync(id);
        if (telemetryLog == null)
        {
            return NotFound();
        }

        _context.TelemetryLogs.Remove(telemetryLog);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> TelemetryLogExists(Guid id)
    {
        return await _context.TelemetryLogs.AnyAsync(e => e.Id == id);
    }
}
