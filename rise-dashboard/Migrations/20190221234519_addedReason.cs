using Microsoft.EntityFrameworkCore.Migrations;

namespace risedashboard.Migrations
{
    public partial class addedReason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "DelegateForms",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "DelegateForms");
        }
    }
}