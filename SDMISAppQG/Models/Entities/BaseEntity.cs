namespace SDMISAppQG.Models.Entities; 

/// <summary>
/// Classe de base pour toutes les entitées
/// </summary>
public abstract class BaseEntity {
   public required Guid Id { get; set; }
   public DateTime CreatedAt { get; set; }
   public DateTime? UpdatedAt { get; set;  }
   public Guid? CreatedBy { get; set; }
}
