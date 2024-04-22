using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomWebShopVN.Migrations
{
    public partial class mymyigrate11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ComentMessage",
                table: "Coments",
                newName: "ProductUserDetail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductUserDetail",
                table: "Coments",
                newName: "ComentMessage");
        }
    }
}
