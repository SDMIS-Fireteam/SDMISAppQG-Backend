namespace SDMISAppQG.Models.Messaging;

/// <summary>
/// Generic message envelope for RabbitMQ messages
/// </summary>
public class MessageEnvelope<T>
{
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
    public string MessageType { get; set; } = typeof(T).Name;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Source { get; set; } = "dotnet-backend";
    public T Data { get; set; } = default!;
}

/// <summary>
/// Example message for vehicle location updates
/// </summary>
public class VehicleLocationUpdate
{
    public required Guid VehicleId { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required DateTime Timestamp { get; set; }
}

/// <summary>
/// Example message for incident notifications
/// </summary>
public class IncidentNotification
{
    public required Guid IncidentId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public required DateTime CreatedAt { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required IncidentTypeInfo Type { get; set; }
}

public class IncidentTypeInfo
{
    public required Guid id { get; set; }
    public required string label { get; set; }
}
