using Microsoft.EntityFrameworkCore.Migrations;

namespace WeighForce.Migrations
{
    public partial class AddWebsiteAndLogo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Office",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Office",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Office");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Office");
        }
    }
}
