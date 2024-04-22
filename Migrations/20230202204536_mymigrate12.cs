using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomWebShopVN.Migrations
{
    public partial class mymigrate12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Coments",
                table: "Coments");

            migrationBuilder.RenameTable(
                name: "Coments",
                newName: "ProductComents");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductComents",
                table: "ProductComents",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductComents",
                table: "ProductComents");

            migrationBuilder.RenameTable(
                name: "ProductComents",
                newName: "Coments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Coments",
                table: "Coments",
                column: "Id");
        }
    }
}
