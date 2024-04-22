using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomWebShopVN.Migrations
{
    public partial class mymigrate18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Discont",
                table: "Orders",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discont",
                table: "Orders");
        }
    }
}
