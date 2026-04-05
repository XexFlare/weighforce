using Microsoft.EntityFrameworkCore.Migrations;

namespace WeighForce.Migrations
{
    public partial class TransporterDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Transporter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Transporter",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Transporter");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Transporter");
        }
    }
}
