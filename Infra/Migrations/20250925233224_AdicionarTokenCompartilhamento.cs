using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PesquisaX.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTokenCompartilhamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TokenCompartilhamento",
                table: "pesquisas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenCompartilhamento",
                table: "pesquisas");
        }
    }
}
