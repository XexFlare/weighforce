using Microsoft.EntityFrameworkCore.Migrations;

namespace WeighForce.Migrations
{
    public partial class EnableDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Weight",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Weight",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "UserMail",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserMail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "TransportInstruction",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TransportInstruction",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Transporter",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Transporter",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Product",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Product",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "OsrData",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OsrData",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Office",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Office",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Inventory",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Inventory",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Dispatch",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Dispatch",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Country",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Country",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Contract",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Contract",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Client",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Client",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Booking",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Booking",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Weight");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Weight");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "UserMail");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserMail");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TransportInstruction");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TransportInstruction");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Transporter");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Transporter");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "OsrData");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OsrData");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Office");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Office");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Dispatch");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Dispatch");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Contract");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Contract");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Booking");
        }
    }
}
