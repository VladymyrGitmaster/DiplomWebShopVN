using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomWebShopVN.Migrations
{
    public partial class mymigrate13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ProductComents",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ProductComents_UserId",
                table: "ProductComents",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductComents_AspNetUsers_UserId",
                table: "ProductComents",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductComents_AspNetUsers_UserId",
                table: "ProductComents");

            migrationBuilder.DropIndex(
                name: "IX_ProductComents_UserId",
                table: "ProductComents");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ProductComents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
