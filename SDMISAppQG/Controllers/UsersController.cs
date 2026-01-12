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
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(AppDbContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Récupère tous les utilisateurs (y compris les utilisateurs supprimés)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserEntity>>> GetUsers([FromQuery] bool includeDeleted = false)
    {
        var query = _context.Users.AsQueryable();

        if (!includeDeleted)
        {
            query = query.Where(u => !u.IsDeleted);
        }

        return await query.ToListAsync();
    }

    /// <summary>
    /// Récupère un utilisateur par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserEntity>> GetUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null || user.IsDeleted)
        {
            return NotFound();
        }

        return user;
    }

    /// <summary>
    /// Récupère un utilisateur par son email
    /// </summary>
    [HttpGet("email/{email}")]
    public async Task<ActionResult<UserEntity>> GetUserByEmail(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    /// <summary>
    /// Récupère un utilisateur par son KeycloakId
    /// </summary>
    [HttpGet("keycloak/{keycloakId}")]
    public async Task<ActionResult<UserEntity>> GetUserByKeycloakId(Guid keycloakId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.KeyCloakId == keycloakId && !u.IsDeleted);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    /// <summary>
    /// Crée un nouvel utilisateur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserEntity>> CreateUser(CreateUserDto dto)
    {
        // Vérifier si l'email existe déjà
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            return BadRequest($"User with email {dto.Email} already exists.");
        }

        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            KeyCloakId = dto.KeyCloakId,
            Email = dto.Email,
            Username = dto.Username,
            Firstname = dto.Firstname,
            Lastname = dto.Lastname,
            Role = dto.Role,
            IsDeleted = false
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    /// <summary>
    /// Met à jour un utilisateur existant
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null || user.IsDeleted)
        {
            return NotFound();
        }

        if (dto.KeyCloakId.HasValue)
            user.KeyCloakId = dto.KeyCloakId.Value;

        if (!string.IsNullOrEmpty(dto.Email))
        {
            // Vérifier si le nouvel email existe déjà
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
            {
                return BadRequest($"User with email {dto.Email} already exists.");
            }
            user.Email = dto.Email;
        }

        if (!string.IsNullOrEmpty(dto.Username))
            user.Username = dto.Username;
        if (!string.IsNullOrEmpty(dto.Firstname))
            user.Firstname = dto.Firstname;
        if (!string.IsNullOrEmpty(dto.Lastname))
            user.Lastname = dto.Lastname;
        if (dto.Role.HasValue)
            user.Role = dto.Role.Value;
        if (dto.IsDeleted.HasValue)
            user.IsDeleted = dto.IsDeleted.Value;

        user.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Users.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Supprime (soft delete) un utilisateur
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null || user.IsDeleted)
        {
            return NotFound();
        }

        // Soft delete
        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Supprime définitivement un utilisateur (hard delete)
    /// </summary>
    [HttpDelete("{id}/permanent")]
    public async Task<IActionResult> PermanentDeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Restaure un utilisateur supprimé
    /// </summary>
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> RestoreUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (!user.IsDeleted)
        {
            return BadRequest("User is not deleted.");
        }

        user.IsDeleted = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
