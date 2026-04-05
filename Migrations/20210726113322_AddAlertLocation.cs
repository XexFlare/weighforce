using Microsoft.EntityFrameworkCore.Migrations;

namespace WeighForce.Migrations
{
    public partial class AddAlertLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OfficeId",
                table: "UserMail",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserMail_OfficeId",
                table: "UserMail",
                column: "OfficeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserMail_OfficeId",
                table: "UserMail");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "UserMail");
        }
    }
}
