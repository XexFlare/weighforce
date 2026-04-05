using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace WeighForce.Migrations
{
    public partial class AddOSRData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OsrData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    cId = table.Column<long>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    gdnId = table.Column<long>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Product = table.Column<string>(nullable: true),
                    Vessel = table.Column<string>(nullable: true),
                    Wagon = table.Column<string>(nullable: true),
                    Tare = table.Column<double>(nullable: false),
                    Gross = table.Column<double>(nullable: false),
                    CRMReleaseNo = table.Column<string>(nullable: true),
                    Warehouse = table.Column<string>(nullable: true),
                    TeamStats = table.Column<string>(nullable: true),
                    Bl = table.Column<string>(nullable: true),
                    GdnValis = table.Column<string>(nullable: true),
                    WagonType = table.Column<string>(nullable: true),
                    SealNumber = table.Column<string>(nullable: true),
                    Bags = table.Column<int>(nullable: false),
                    Unit = table.Column<double>(nullable: false),
                    ProductWeight = table.Column<double>(nullable: false),
                    RailedAtTrain = table.Column<string>(nullable: true),
                    ArrivalDate = table.Column<DateTime>(nullable: false),
                    BagsReceived = table.Column<int>(nullable: false),
                    Wet = table.Column<double>(nullable: false),
                    QtyReceived = table.Column<double>(nullable: false),
                    Diff = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsrData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OsrData");
        }
    }
}
