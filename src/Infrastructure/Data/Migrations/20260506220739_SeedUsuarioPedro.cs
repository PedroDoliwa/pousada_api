using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsuarioPedro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Mesmo esquema de hash que AuthService (SHA256 + Base64 da senha em UTF-8). Senha: 123456
            migrationBuilder.Sql(
                """
                INSERT INTO "Usuarios" ("Nome", "Email", "SenhaHash", "Perfil")
                SELECT 'Pedro', 'pedro@gmail.com', 'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', 'Gerente'
                WHERE NOT EXISTS (SELECT 1 FROM "Usuarios" WHERE "Email" = 'pedro@gmail.com');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """DELETE FROM "Usuarios" WHERE "Email" = 'pedro@gmail.com';""");
        }
    }
}
