using Microsoft.EntityFrameworkCore.Migrations;

namespace WeighForce.Migrations
{
    public partial class AddSyncVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Weight",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "UserMail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "TransportInstruction",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Transporter",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Product",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "OsrData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Office",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Inventory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Dispatch",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Country",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Contract",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Client",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Booking",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Weight");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "UserMail");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "TransportInstruction");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Transporter");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "OsrData");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Office");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Dispatch");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Contract");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Booking");
        }
    }
}
