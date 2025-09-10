using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PesquisaX.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_ultimo_acesso",
                table: "usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "token",
                table: "usuarios",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "data_ultimo_acesso",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "token",
                table: "usuarios");
        }
    }
}
