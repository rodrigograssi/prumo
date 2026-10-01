using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prumo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarNomeDoResultadoDesejado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "nome_do_resultado_desejado",
                table: "pesquisas",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "Resultado Desejado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nome_do_resultado_desejado",
                table: "pesquisas");
        }
    }
}
