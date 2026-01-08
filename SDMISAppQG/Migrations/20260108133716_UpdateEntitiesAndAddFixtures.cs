using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SDMISAppQG.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntitiesAndAddFixtures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_IncidentTypes_TypeId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_TypeId",
                table: "Incidents");

            migrationBuilder.AddColumn<List<string>>(
                name: "Consumables",
                table: "VehicleTypes",
                type: "text[]",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UnavailabilityReason",
                table: "Vehicles",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Consumable",
                table: "Vehicles",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassengerCount",
                table: "Vehicles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Assignees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InterventionId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Itinerary = table.Column<string>(type: "jsonb", nullable: true),
                    Begin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignees", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "IncidentTypes",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Label", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Feu", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Incendie", null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Circulation", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Accident de la route", null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Médical", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secours à personne", null }
                });

            migrationBuilder.InsertData(
                table: "Incidents",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "Location", "Priority", "Severity", "Source", "Status", "TypeId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Incendie d'appartement - 3ème étage", (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (2.3522 48.8566)"), 8.5f, 2, 0, 0, new Guid("11111111-1111-1111-1111-111111111111"), null },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Accident de la circulation - 2 véhicules impliqués", (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (2.36 48.857)"), 5f, 1, 1, 1, new Guid("22222222-2222-2222-2222-222222222222"), null }
                });

            migrationBuilder.InsertData(
                table: "VehicleTypes",
                columns: new[] { "Id", "Consumables", "CreatedAt", "CreatedBy", "CrewCapacity", "Label", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444444"), new List<string> { "water" }, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 6, "Camion Pompe", null },
                    { new Guid("66666666-6666-6666-6666-666666666666"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, "VSAV (Ambulance)", null }
                });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "Availability", "Consumable", "CreatedAt", "CreatedBy", "Fuel", "IdHardware", "LastLocation", "PassengerCount", "TypeId", "UnavailabilityReason", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777777"), 0, "{\"water\": 1000, \"foam\": 200}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 85.5f, 1001, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (2.3522 48.8566)"), 6, new Guid("44444444-4444-4444-4444-444444444444"), null, null },
                    { new Guid("88888888-8888-8888-8888-888888888888"), 0, "{\"water\": 500}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 72f, 1002, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (2.35 48.855)"), 4, new Guid("55555555-5555-5555-5555-555555555555"), null, null },
                    { new Guid("99999999-9999-9999-9999-999999999999"), 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 60.5f, 1003, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (2.34 48.86)"), 3, new Guid("66666666-6666-6666-6666-666666666666"), null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignees");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "IncidentTypes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "IncidentTypes",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "IncidentTypes",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Incidents",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "Incidents",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));

            migrationBuilder.DropColumn(
                name: "Consumables",
                table: "VehicleTypes");

            migrationBuilder.DropColumn(
                name: "Consumable",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "PassengerCount",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "UnavailabilityReason",
                table: "Vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_TypeId",
                table: "Incidents",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_IncidentTypes_TypeId",
                table: "Incidents",
                column: "TypeId",
                principalTable: "IncidentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
