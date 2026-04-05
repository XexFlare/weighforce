using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeighForce.Migrations
{
    public partial class AddBranches : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TicketNumber",
                table: "Weight",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contacts",
                table: "Office",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TicketPrefix",
                table: "Office",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Branch",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    cId = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    SyncVersion = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    OfficeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branch_Office_OfficeId",
                        column: x => x.OfficeId,
                        principalTable: "Office",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branch_OfficeId",
                table: "Branch",
                column: "OfficeId");

            migrationBuilder.AddColumn<long>(
                name: "BranchId",
                table: "Booking",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_BranchId",
                table: "Booking",
                column: "BranchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Branch");

            migrationBuilder.DropColumn(
                name: "TicketNumber",
                table: "Weight");

            migrationBuilder.DropColumn(
                name: "Contacts",
                table: "Office");

            migrationBuilder.DropColumn(
                name: "TicketPrefix",
                table: "Office");
            migrationBuilder.DropIndex(
                name: "IX_Booking_BranchId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Booking");
        }
    }
}
