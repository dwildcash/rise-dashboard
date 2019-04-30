using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace risedashboard.Migrations
{
    public partial class RollHundred : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RollHundredRecords",
                columns: table => new
                {
                    RollHundredRecordId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Time = table.Column<DateTime>(nullable: false),
                    Options = table.Column<int>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    PickedNumber = table.Column<int>(nullable: false),
                    LuckyNumber = table.Column<int>(nullable: false),
                    Multiplier = table.Column<double>(nullable: false),
                    AmountPaid = table.Column<double>(nullable: false),
                    Winner = table.Column<bool>(nullable: false),
                    TransactionResult = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RollHundredRecords", x => x.RollHundredRecordId);
                    table.ForeignKey(
                        name: "FK_RollHundredRecords_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RollHundredRecords_UserId",
                table: "RollHundredRecords",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RollHundredRecords");
        }
    }
}
