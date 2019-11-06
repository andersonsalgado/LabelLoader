using Microsoft.EntityFrameworkCore.Migrations;

namespace GeekBurger.LabelLoader.Migrations.Migrations
{
    public partial class Iniciandobanco : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cad_Ingredientes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Descricao = table.Column<string>(type: "varchar(60)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cad_Ingredientes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cad_Ingredientes");
        }
    }
}
