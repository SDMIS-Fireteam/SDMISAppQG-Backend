using System.ComponentModel.DataAnnotations.Schema;

namespace SDMISAppQG.Models.Entities;

public class PassengerEntity : BaseEntity {
   public required Guid VehicleId { get; set; }

   [ForeignKey(nameof(VehicleId))]
   public required VehicleEntity Vehicle { get; set; }

   public required Guid UserId { get; set; }

   [ForeignKey(nameof(UserId))]
   public required UserEntity User { get; set; }
}
