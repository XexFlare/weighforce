using Microsoft.EntityFrameworkCore.Migrations;

namespace WeighForce.Migrations
{
    public partial class AddLpoAndDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Booking",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LPO",
                table: "Booking",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "LPO",
                table: "Booking");
        }
    }
}
