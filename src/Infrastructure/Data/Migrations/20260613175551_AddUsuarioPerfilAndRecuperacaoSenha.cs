using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioPerfilAndRecuperacaoSenha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Foto",
                table: "Usuarios",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoContentType",
                table: "Usuarios",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UsuarioRecuperacaoSenhas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExpiraEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRecuperacaoSenhas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioRecuperacaoSenhas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRecuperacaoSenhas_TokenHash",
                table: "UsuarioRecuperacaoSenhas",
                column: "TokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRecuperacaoSenhas_UsuarioId",
                table: "UsuarioRecuperacaoSenhas",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioRecuperacaoSenhas");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FotoContentType",
                table: "Usuarios");
        }
    }
}
