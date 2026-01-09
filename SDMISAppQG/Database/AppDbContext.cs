using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NetTopologySuite.Geometries;
using SDMISAppQG.Models.Entities;
using SDMISAppQG.Models.Enums;
using SDMISAppQG.Models.Enums.Incidents;
using SDMISAppQG.Models.Enums.Vehicle;

namespace SDMISAppQG.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

    public DbSet<VehicleEntity> Vehicles { get; set; }
    public DbSet<IncidentEntity> Incidents { get; set; }
    public DbSet<InterventionEntity> Interventions { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<IncidentTypeEntity> IncidentTypes { get; set; }
    public DbSet<VehicleTypeEntity> VehicleTypes { get; set; }
    public DbSet<TelemetryLogsEntity> TelemetryLogs { get; set; }
    public DbSet<Assigned> Assignees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration des relations avec shadow properties
        modelBuilder.Entity<VehicleEntity>()
            .HasOne(v => v.Type)
            .WithMany()
            .HasForeignKey("TypeId")
            .IsRequired();

        modelBuilder.Entity<IncidentEntity>()
            .HasOne(i => i.Type)
            .WithMany()
            .HasForeignKey("TypeId")
            .IsRequired();

        // Seed IncidentTypes
        var incidentType1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var incidentType2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var incidentType3 = Guid.Parse("33333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<IncidentTypeEntity>().HasData(
            new IncidentTypeEntity
            {
                Id = incidentType1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Label = "Incendie",
                Category = "Feu"
            },
            new IncidentTypeEntity
            {
                Id = incidentType2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Label = "Accident de la route",
                Category = "Circulation"
            },
            new IncidentTypeEntity
            {
                Id = incidentType3,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Label = "Secours à personne",
                Category = "Médical"
            }
        );

        // Seed VehicleTypes
        var vehicleType1 = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var vehicleType2 = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var vehicleType3 = Guid.Parse("66666666-6666-6666-6666-666666666666");

        modelBuilder.Entity<VehicleTypeEntity>().HasData(
            new VehicleTypeEntity
            {
                Id = vehicleType1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Label = "Camion Pompe",
                CrewCapacity = 6,
                Consumables = new List<string> { "water"}
            },
            new VehicleTypeEntity
            {
                Id = vehicleType2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Label = "FPT (Fourgon Pompe Tonne)",
                CrewCapacity = 4,
                Consumables = new List<string> { "water" }
            },
            new VehicleTypeEntity
            {
                Id = vehicleType3,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Label = "VSAV (Ambulance)",
                CrewCapacity = 3,
            }
        );

        // Seed Vehicles
        var vehicle1 = Guid.Parse("77777777-7777-7777-7777-777777777777");
        var vehicle2 = Guid.Parse("88888888-8888-8888-8888-888888888888");
        var vehicle3 = Guid.Parse("99999999-9999-9999-9999-999999999999");

        modelBuilder.Entity<VehicleEntity>().HasData(
            new
            {
                Id = vehicle1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IdHardware = 1,
                TypeId = vehicleType1,
                LastLocation = new Point(4.8183093032891975, 45.7774230183619) { SRID = 4326 },
                Availability = VehicleAvailability.Available,
                UnavailabilityReason = (VehicleUnavailabilityReason?)null,
                Fuel = 85.5f,
                Consumable = "{\"water\": \"100%\", \"foam\": \"80%\"}",
                PassengerCount = (int?)6
            },
            new
            {
                Id = vehicle2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IdHardware = 2,
                TypeId = vehicleType2,
                LastLocation = new Point(4.874379168171828, 45.76148618959854) { SRID = 4326 },
                Availability = VehicleAvailability.Available,
                UnavailabilityReason = (VehicleUnavailabilityReason?)null,
                Fuel = 72.0f,
                Consumable = "{\"water\": \"100%\"}",
                PassengerCount = (int?)4
            },
            new
            {
                Id = vehicle3,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IdHardware = 3,
                TypeId = vehicleType3,
                LastLocation = new Point(4.855804926195958, 45.74707280399255) { SRID = 4326 },
                Availability = VehicleAvailability.Ongoing,
                UnavailabilityReason = (VehicleUnavailabilityReason?)null,
                Fuel = 60.5f,
                Consumable = (string?)null,
                PassengerCount = (int?)3
            }
        );

        // Seed Incidents
        var incident1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var incident2 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        modelBuilder.Entity<IncidentEntity>().HasData(
            new
            {
                Id = incident1,
                TypeId = incidentType1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Location = new Point(2.3522, 48.8566) { SRID = 4326 },
                Severity = IncidentSeverity.High,
                Priority = 8.5f,
                Status = IncidentStatus.Declared,
                Source = IncidentSource.External,
                Description = "Incendie d'appartement - 3ème étage",
            },
            new
            {
                Id = incident2,
                TypeId = incidentType2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Location = new Point(2.3600, 48.8570) { SRID = 4326 },
                Severity = IncidentSeverity.Medium,
                Priority = 5.0f,
                Status = IncidentStatus.Ongoing,
                Source = IncidentSource.Internal,
                Description = "Accident de la circulation - 2 véhicules impliqués",
            }
        );
    }

    public override int SaveChanges()
    {
        GenerateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        GenerateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Gère l'application automatique de la date de création et de modification des entitées
    /// </summary>
    private void GenerateTimestamps()
    {
        IEnumerable<EntityEntry> entities = ChangeTracker
           .Entries()
           .Where(e => e.Entity is BaseEntity &&
              (e.State == EntityState.Added) || (e.State == EntityState.Modified));
        foreach (EntityEntry entityEntry in entities)
        {
            BaseEntity entity = (BaseEntity)entityEntry.Entity;
            if (entityEntry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}
