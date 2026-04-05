using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace WeighForce.Migrations
{
    public partial class AddTransporters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Closed",
                table: "TransportInstruction",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Inventory",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "TransporterId",
                table: "Booking",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Transporter",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    cId = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    CountryId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transporter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transporter_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_TransporterId",
                table: "Booking",
                column: "TransporterId");

            migrationBuilder.CreateIndex(
                name: "IX_Transporter_CountryId",
                table: "Transporter",
                column: "CountryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transporter");

            migrationBuilder.DropIndex(
                name: "IX_Booking_TransporterId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "Closed",
                table: "TransportInstruction");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "TransporterId",
                table: "Booking");

            migrationBuilder.AddColumn<string>(
                name: "Transporter",
                table: "Booking",
                type: "text",
                nullable: true);
        }
    }
}
