using Microsoft.EntityFrameworkCore.Migrations;

namespace risedashboard.Migrations
{
    public partial class renamecolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EncryptedBip39",
                table: "AspNetUsers",
                newName: "Secret");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Secret",
                table: "AspNetUsers",
                newName: "EncryptedBip39");
        }
    }
}
