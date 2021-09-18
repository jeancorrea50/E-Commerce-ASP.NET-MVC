using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Portifolio675.Migrations
{
    public partial class v4564564 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeProduto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnoFabricacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Imagem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeCategoria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoProduto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modelo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProdId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categs_Prods_ProdId",
                        column: x => x.ProdId,
                        principalTable: "Prods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categs_ProdId",
                table: "Categs",
                column: "ProdId");

            migrationBuilder.CreateIndex(
                name: "IX_Prods_CategId",
                table: "Prods",
                column: "CategId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prods_Categs_CategId",
                table: "Prods",
                column: "CategId",
                principalTable: "Categs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categs_Prods_ProdId",
                table: "Categs");

            migrationBuilder.DropTable(
                name: "Prods");

            migrationBuilder.DropTable(
                name: "Categs");
        }
    }
}
