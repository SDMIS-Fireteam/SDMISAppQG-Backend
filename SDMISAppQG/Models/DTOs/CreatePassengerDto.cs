using System;

namespace SDMISAppQG.Models.DTOs;

public class CreatePassengerDto
{
    public required Guid VehicleId { get; set; }
    public required Guid UserId { get; set; }
}
