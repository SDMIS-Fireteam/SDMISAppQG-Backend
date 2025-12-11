namespace SDMISAppQG.Models; 
public abstract class BaseEntity {
   public required Guid Id { get; set; }
   public DateTime CreatedAt { get; set; }
   public DateTime? UpdatedAt { get; set;  }
   public required Guid CreatedBy { get; set; }
}
