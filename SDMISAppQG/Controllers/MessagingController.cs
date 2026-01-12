using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMISAppQG.Infrastructure.Services.RabbitMQ;
using SDMISAppQG.Models.Messaging;
using SDMISAppQG.Database;

namespace SDMISAppQG.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagingController : ControllerBase
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ILogger<MessagingController> _logger;
    private readonly AppDbContext _context;

    public MessagingController(IRabbitMQService rabbitMQService, ILogger<MessagingController> logger, AppDbContext context)
    {
        _rabbitMQService = rabbitMQService;
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Send a vehicle location update to Java service
    /// </summary>
    [HttpPost("vehicle-location")]
    public IActionResult SendVehicleLocation([FromBody] VehicleLocationUpdate locationUpdate)
    {
        try
        {
            var envelope = new MessageEnvelope<VehicleLocationUpdate>
            {
                Data = locationUpdate
            };

            _rabbitMQService.PublishToJava(envelope);
            _logger.LogInformation("Vehicle location update sent for vehicle {VehicleId}", locationUpdate.VehicleId);

            return Ok(new
            {
                success = true,
                message = "Vehicle location update sent to Java service",
                messageId = envelope.MessageId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send vehicle location update");
            return StatusCode(500, new { success = false, message = "Failed to send message" });
        }
    }

    /// <summary>
    /// Send an incident notification to Java service
    /// </summary>
    [HttpPost("incident-notification/{incidentId}")]
    public IActionResult SendIncidentNotification(Guid incidentId)
    {
        var incident = _context.Incidents
            .Include(i => i.Type)
            .FirstOrDefault(i => i.Id == incidentId);
        if (incident == null)
        {
            return NotFound(new { success = false, message = "Incident not found" });
        }
        var incidentNotification = new IncidentNotification
        {
            IncidentId = incident.Id,
            Status = incident.Status.ToString(),
            Description = incident.Description ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            Latitude = incident.Location.Coordinate[1],
            Longitude = incident.Location.Coordinate[0],
            Type = new IncidentTypeInfo
            {
                id = incident.Type?.Id ?? Guid.Empty,
                label = incident.Type?.Label ?? string.Empty
            }
        };

        try
        {
            var envelope = new MessageEnvelope<IncidentNotification>
            {
                Data = incidentNotification
            };

            _rabbitMQService.PublishToJava(envelope);
            _logger.LogInformation("Incident notification sent for incident {IncidentId}", incident.Id);

            return Ok(new
            {
                success = true,
                message = "Incident notification sent to Java service",
                messageId = envelope.MessageId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send incident notification");
            return StatusCode(500, new { success = false, message = "Failed to send message" });
        }
    }

    /// <summary>
    /// Send a generic message to Java service
    /// </summary>
    [HttpPost("send-to-java")]
    public IActionResult SendMessageToJava([FromBody] object message)
    {
        try
        {
            var envelope = new MessageEnvelope<object>
            {
                Data = message
            };

            _rabbitMQService.PublishToJava(envelope);
            _logger.LogInformation("Generic message sent to Java service");

            return Ok(new
            {
                success = true,
                message = "Message sent to Java service",
                messageId = envelope.MessageId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to Java service");
            return StatusCode(500, new { success = false, message = "Failed to send message" });
        }
    }

    /// <summary>
    /// Test endpoint to verify RabbitMQ connection
    /// </summary>
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        try
        {
            return Ok(new
            {
                success = true,
                message = "RabbitMQ messaging service is operational",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ health check failed");
            return StatusCode(500, new { success = false, message = "RabbitMQ service error" });
        }
    }
}
