using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeighForce.Migrations
{
    public partial class AddLogAndTrackTIChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TIChange",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(nullable: true),
                    BookingId = table.Column<long>(nullable: true),
                    OldValueId = table.Column<long>(nullable: true),
                    NewValueId = table.Column<long>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TIChange_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TIChange_TransportInstruction_NewValueId",
                        column: x => x.NewValueId,
                        principalTable: "TransportInstruction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TIChange_TransportInstruction_OldValueId",
                        column: x => x.OldValueId,
                        principalTable: "TransportInstruction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TIChange_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(nullable: true),
                    Resource = table.Column<string>(nullable: true),
                    ResourceId = table.Column<long>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventLog_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TIChange_BookingId",
                table: "TIChange",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_TIChange_NewValueId",
                table: "TIChange",
                column: "NewValueId");

            migrationBuilder.CreateIndex(
                name: "IX_TIChange_OldValueId",
                table: "TIChange",
                column: "OldValueId");

            migrationBuilder.CreateIndex(
                name: "IX_TIChange_UserId",
                table: "TIChange",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_UserId",
                table: "EventLog",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TIChange");

            migrationBuilder.DropTable(
                name: "EventLog");
        }
    }
}
