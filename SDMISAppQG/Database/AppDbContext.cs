using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SDMISAppQG.Models.Entities;

namespace SDMISAppQG.Database; 
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {

   public override int SaveChanges() {
      GenerateTimestamps();
      return base.SaveChanges();
   }

   public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
      GenerateTimestamps();
      return base.SaveChangesAsync(cancellationToken);
   }

   /// <summary>
   /// Gère l'application automatique de la date de création et de modification des entitées
   /// </summary>
   private void GenerateTimestamps() {
      IEnumerable<EntityEntry> entities = ChangeTracker
         .Entries()
         .Where(e => e.Entity is BaseEntity &&
            (e.State == EntityState.Added) || (e.State == EntityState.Modified));
      foreach (EntityEntry entityEntry in entities) {
         BaseEntity entity = (BaseEntity)entityEntry.Entity;
         if (entityEntry.State == EntityState.Added) {
            entity.CreatedAt = DateTime.UtcNow;
         }
         entity.UpdatedAt = DateTime.UtcNow;
      }
   }
}
