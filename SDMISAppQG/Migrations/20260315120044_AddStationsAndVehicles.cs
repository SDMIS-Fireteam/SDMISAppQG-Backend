using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SDMISAppQG.Migrations
{
    /// <inheritdoc />
    public partial class AddStationsAndVehicles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<Point>(type: "geography(Point, 4326)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Location", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (4.82 45.74)"), "Caserne Lyon-Confluence", null },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (4.86 45.76)"), "Caserne Lyon-Part-Dieu", null },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (4.83 45.77)"), "Caserne Villeurbanne", null }
                });

            migrationBuilder.UpdateData(
                table: "VehicleTypes",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "Consumables",
                value: new List<string> { "water" });

            migrationBuilder.UpdateData(
                table: "VehicleTypes",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "Consumables",
                value: new List<string> { "water" });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "Availability", "Consumable", "CreatedAt", "CreatedBy", "Fuel", "IdHardware", "LastLocation", "PassengerCount", "TypeId", "UnavailabilityReason", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444444"), 0, "{\"water\": \"100%\", \"foam\": \"100%\"}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 100f, 4, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (4.82 45.74)"), 6, new Guid("44444444-4444-4444-4444-444444444444"), null, null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), 0, "{\"water\": \"100%\", \"foam\": \"100%\"}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 100f, 5, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (4.86 45.76)"), 4, new Guid("55555555-5555-5555-5555-555555555555"), null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.UpdateData(
                table: "VehicleTypes",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "Consumables",
                value: new List<string> { "water" });

            migrationBuilder.UpdateData(
                table: "VehicleTypes",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "Consumables",
                value: new List<string> { "water" });
        }
    }
}
