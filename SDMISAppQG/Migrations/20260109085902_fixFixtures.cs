using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDMISAppQG.Migrations
{
    /// <inheritdoc />
    public partial class fixFixtures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "VehicleTypes",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "Consumables",
                value: new List<string> { "water" });

            migrationBuilder.InsertData(
                table: "VehicleTypes",
                columns: new[] { "Id", "CreatedAt", "Label", "CrewCapacity", "Consumables" },
                values: new object[] { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), "Camion Citerne", 4, new List<string> { "water" } }
                );

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "Consumable",
                value: "{\"water\": \"100%\", \"foam\": \"80%\"}");

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                column: "Consumable",
                value: "{\"water\": \"100%\"}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                column: "Consumable",
                value: "{\"water\": 1000, \"foam\": 200}");

            migrationBuilder.UpdateData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                column: "Consumable",
                value: "{\"water\": 500}");
        }
    }
}
