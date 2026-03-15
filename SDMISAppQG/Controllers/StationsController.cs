using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMISAppQG.Database;
using SDMISAppQG.Models.Entities;

namespace SDMISAppQG.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StationsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère toutes les casernes
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StationEntity>>> GetStations()
    {
        return await _context.Stations.ToListAsync();
    }

    /// <summary>
    /// Récupère une caserne par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<StationEntity>> GetStation(Guid id)
    {
        var station = await _context.Stations.FindAsync(id);

        if (station == null)
        {
            return NotFound();
        }

        return station;
    }

    /// <summary>
    /// Crée une nouvelle caserne
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<StationEntity>> CreateStation(StationEntity station)
    {
        station.Id = Guid.NewGuid();
        station.CreatedAt = DateTime.UtcNow;

        _context.Stations.Add(station);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStation), new { id = station.Id }, station);
    }

    /// <summary>
    /// Met à jour une caserne existante
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStation(Guid id, StationEntity station)
    {
        if (id != station.Id)
        {
            return BadRequest();
        }

        _context.Entry(station).State = EntityState.Modified;
        station.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await StationExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    /// <summary>
    /// Supprime une caserne
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStation(Guid id)
    {
        var station = await _context.Stations.FindAsync(id);
        if (station == null)
        {
            return NotFound();
        }

        _context.Stations.Remove(station);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> StationExists(Guid id)
    {
        return await _context.Stations.AnyAsync(e => e.Id == id);
    }
}
