using Microsoft.EntityFrameworkCore.Migrations;

namespace Portifolio675.Migrations
{
    public partial class v20214 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Imagem",
                table: "Prods",
                newName: "ProfileImage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImage",
                table: "Prods",
                newName: "Imagem");
        }
    }
}
