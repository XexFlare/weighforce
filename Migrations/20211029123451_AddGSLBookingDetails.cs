using Microsoft.EntityFrameworkCore.Migrations;

namespace WeighForce.Migrations
{
    public partial class AddGSLBookingDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryNoteNumber",
                table: "Booking",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoadingAuthorityNumber",
                table: "Booking",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherTransporter",
                table: "Booking",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryNoteNumber",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "LoadingAuthorityNumber",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "OtherTransporter",
                table: "Booking");
        }
    }
}
