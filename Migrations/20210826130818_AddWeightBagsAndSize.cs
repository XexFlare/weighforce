using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeighForce.Migrations
{
    public partial class AddWeightBagsAndSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Bags",
                table: "Weight",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Weight",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Printed",
                table: "Weight",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "KineticRef",
                table: "TransportInstruction",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bags",
                table: "Weight");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Weight");

            migrationBuilder.DropColumn(
                name: "Printed",
                table: "Weight");

            migrationBuilder.DropColumn(
                name: "KineticRef",
                table: "TransportInstruction");
        }
    }
}