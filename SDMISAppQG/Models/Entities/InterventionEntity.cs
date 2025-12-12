using SDMISAppQG.Models.Enums;

namespace SDMISAppQG.Models.Entities; 
public class InterventionEntity : BaseEntity {
   public Guid IncidentId { get; set; }
   public DateTime Begin { get; set; }
   public DateTime End { get; set; }
   public InterventionStatus Status { get; set; }
   public DateTime? ConfirmedAt { get; set; }
   public Guid? ConfirmedBy { get; set; }
}
