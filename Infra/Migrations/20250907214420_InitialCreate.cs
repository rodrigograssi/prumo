using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PesquisaX.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pesquisas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pesquisas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    senha = table.Column<string>(type: "text", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "grupos_questoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    categoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false, defaultValue: "#FF6B6B"),
                    pesquisa_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grupos_questoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_grupos_questoes_pesquisas_pesquisa_id",
                        column: x => x.pesquisa_id,
                        principalTable: "pesquisas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resultados_pessoas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome_pessoa = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    data_avaliacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    pesquisa_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resultados_pessoas", x => x.id);
                    table.ForeignKey(
                        name: "FK_resultados_pessoas_pesquisas_pesquisa_id",
                        column: x => x.pesquisa_id,
                        principalTable: "pesquisas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "questoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    texto = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    e_invertida = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    grupo_questao_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_questoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_questoes_grupos_questoes_grupo_questao_id",
                        column: x => x.grupo_questao_id,
                        principalTable: "grupos_questoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "respostas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    resultado_pessoa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    questao_id = table.Column<Guid>(type: "uuid", nullable: false),
                    valor_bruto = table.Column<int>(type: "integer", nullable: false),
                    valor_processado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_respostas", x => x.id);
                    table.ForeignKey(
                        name: "FK_respostas_questoes_questao_id",
                        column: x => x.questao_id,
                        principalTable: "questoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_respostas_resultados_pessoas_resultado_pessoa_id",
                        column: x => x.resultado_pessoa_id,
                        principalTable: "resultados_pessoas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_grupos_questoes_pesquisa_id",
                table: "grupos_questoes",
                column: "pesquisa_id");

            migrationBuilder.CreateIndex(
                name: "IX_questoes_grupo_questao_id",
                table: "questoes",
                column: "grupo_questao_id");

            migrationBuilder.CreateIndex(
                name: "IX_respostas_questao_id",
                table: "respostas",
                column: "questao_id");

            migrationBuilder.CreateIndex(
                name: "IX_respostas_resultado_pessoa_id",
                table: "respostas",
                column: "resultado_pessoa_id");

            migrationBuilder.CreateIndex(
                name: "IX_resultados_pessoas_pesquisa_id",
                table: "resultados_pessoas",
                column: "pesquisa_id");

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "respostas");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "questoes");

            migrationBuilder.DropTable(
                name: "resultados_pessoas");

            migrationBuilder.DropTable(
                name: "grupos_questoes");

            migrationBuilder.DropTable(
                name: "pesquisas");
        }
    }
}
