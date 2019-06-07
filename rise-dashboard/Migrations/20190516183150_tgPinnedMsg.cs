using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace risedashboard.Migrations
{
    public partial class tgPinnedMsg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RollHundredRecords");

            migrationBuilder.CreateTable(
                name: "TgPinnedMsgs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(nullable: false),
                    AppUserId = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TgPinnedMsgs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TgPinnedMsgs_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TgPinnedMsgs_AppUserId",
                table: "TgPinnedMsgs",
                column: "AppUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TgPinnedMsgs");

            migrationBuilder.CreateTable(
                name: "RollHundredRecords",
                columns: table => new
                {
                    RollHundredRecordId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Amount = table.Column<double>(nullable: false),
                    AmountPaid = table.Column<double>(nullable: false),
                    LuckyNumber = table.Column<int>(nullable: false),
                    Multiplier = table.Column<double>(nullable: false),
                    Options = table.Column<int>(nullable: false),
                    PickedNumber = table.Column<int>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    TransactionResult = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Winner = table.Column<bool>(nullable: false)
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
    }
}