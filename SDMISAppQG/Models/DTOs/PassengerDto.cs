using System;

namespace SDMISAppQG.Models.DTOs;

public class PassengerDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
