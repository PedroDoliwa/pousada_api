using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioAndAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ordem: tabela Usuarios e um registro antes da FK — default 0 em Pousadas quebrava a FK (não existe Usuario Id=0).
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    SenhaHash = table.Column<string>(type: "text", nullable: false),
                    Perfil = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.Sql(
                @"INSERT INTO ""Usuarios"" (""Nome"", ""Email"", ""SenhaHash"", ""Perfil"")
VALUES ('Admin (seed)', 'admin-seed@local', 'seed-hash-trocar-no-login', 'Gerente');");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Pousadas",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Pousadas_UsuarioId",
                table: "Pousadas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pousadas_Usuarios_UsuarioId",
                table: "Pousadas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pousadas_Usuarios_UsuarioId",
                table: "Pousadas");

            migrationBuilder.DropIndex(
                name: "IX_Pousadas_UsuarioId",
                table: "Pousadas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Pousadas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
