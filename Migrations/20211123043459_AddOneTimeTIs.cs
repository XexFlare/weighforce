using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeighForce.Migrations
{
    public partial class AddOneTimeTIs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OneTime",
                table: "TransportInstruction",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UnitId",
                table: "Product",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UnitId",
                table: "Office",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Unit",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    cId = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    SyncVersion = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_UnitId",
                table: "Product",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Office_UnitId",
                table: "Office",
                column: "UnitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Unit");

            migrationBuilder.DropIndex(
                name: "IX_Product_UnitId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Office_UnitId",
                table: "Office");

            migrationBuilder.DropColumn(
                name: "OneTime",
                table: "TransportInstruction");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Office");
        }
    }
}
