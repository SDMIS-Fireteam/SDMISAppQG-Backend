using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDMISAppQG.Migrations
{
    /// <inheritdoc />
    public partial class AddIdHardwareToVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdHardware",
                table: "Vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_IdHardware",
                table: "Vehicles",
                column: "IdHardware",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_IdHardware",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "IdHardware",
                table: "Vehicles");
        }
    }
}
