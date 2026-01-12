using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDMISAppQG.Migrations
{
    /// <inheritdoc />
    public partial class ApiAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Vehicles");

            migrationBuilder.AddColumn<Guid>(
                name: "TypeId",
                table: "Vehicles",
                type: "uuid",
                nullable: false
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Vehicles");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
