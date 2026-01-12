using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDMISAppQG.Migrations
{
    /// <inheritdoc />
    public partial class ChangeVehicleProposition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InterventionId",
                table: "VehiclePropositions",
                newName: "IncidentId");

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

            migrationBuilder.CreateIndex(
                name: "IX_VehiclePropositions_IncidentId",
                table: "VehiclePropositions",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_VehiclePropositions_VehicleId",
                table: "VehiclePropositions",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_VehiclePropositions_Incidents_IncidentId",
                table: "VehiclePropositions",
                column: "IncidentId",
                principalTable: "Incidents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehiclePropositions_Vehicles_VehicleId",
                table: "VehiclePropositions",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehiclePropositions_Incidents_IncidentId",
                table: "VehiclePropositions");

            migrationBuilder.DropForeignKey(
                name: "FK_VehiclePropositions_Vehicles_VehicleId",
                table: "VehiclePropositions");

            migrationBuilder.DropIndex(
                name: "IX_VehiclePropositions_IncidentId",
                table: "VehiclePropositions");

            migrationBuilder.DropIndex(
                name: "IX_VehiclePropositions_VehicleId",
                table: "VehiclePropositions");

            migrationBuilder.RenameColumn(
                name: "IncidentId",
                table: "VehiclePropositions",
                newName: "InterventionId");

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
