using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDMISAppQG.Models.Entities;

public class StationEntity : BaseEntity {
   public required string Name { get; set; }
   [Column(TypeName = "geography(Point, 4326)")]
   public required Point Location { get; set; }
}
