namespace SDMISAppQG.Models.Entities;

public class VehicleTypeEntity : BaseEntity
{
    public required string Label { get; set; }
    public required int CrewCapacity { get; set; }
}
