using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PesquisaX.Infra.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirRelacionamentoRespostas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_respostas_resultados_pessoas_resultado_pessoa_id1",
                table: "respostas");

            migrationBuilder.DropIndex(
                name: "IX_respostas_resultado_pessoa_id1",
                table: "respostas");

            migrationBuilder.CreateIndex(
                name: "IX_respostas_resultado_pessoa_id",
                table: "respostas",
                column: "resultado_pessoa_id");

            migrationBuilder.AddForeignKey(
                name: "FK_respostas_resultados_pessoas_resultado_pessoa_id",
                table: "respostas",
                column: "resultado_pessoa_id",
                principalTable: "resultados_pessoas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_respostas_resultados_pessoas_resultado_pessoa_id",
                table: "respostas");

            migrationBuilder.DropIndex(
                name: "IX_respostas_resultado_pessoa_id",
                table: "respostas");

            migrationBuilder.CreateIndex(
                name: "IX_respostas_resultado_pessoa_id1",
                table: "respostas",
                column: "resultado_pessoa_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_respostas_resultados_pessoas_resultado_pessoa_id1",
                table: "respostas",
                column: "resultado_pessoa_id1",
                principalTable: "resultados_pessoas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
