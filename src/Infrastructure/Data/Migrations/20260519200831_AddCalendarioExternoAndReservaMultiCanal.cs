using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCalendarioExternoAndReservaMultiCanal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservas_QuartoId",
                table: "Reservas");

            migrationBuilder.AddColumn<int>(
                name: "CalendarioExternoId",
                table: "Reservas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Origem",
                table: "Reservas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Manual");

            migrationBuilder.AddColumn<DateTime>(
                name: "SincronizadoEm",
                table: "Reservas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TituloExterno",
                table: "Reservas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UidExterno",
                table: "Reservas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenExportacao",
                table: "Quartos",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE "Quartos"
                SET "TokenExportacao" = replace(gen_random_uuid()::text, '-', '')
                WHERE "TokenExportacao" = '';
                """);

            migrationBuilder.CreateTable(
                name: "CalendariosExternos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuartoId = table.Column<int>(type: "integer", nullable: false),
                    Canal = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UrlImportacao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    UltimaSincronizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UltimoErro = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendariosExternos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendariosExternos_Quartos_QuartoId",
                        column: x => x.QuartoId,
                        principalTable: "Quartos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_CalendarioExternoId",
                table: "Reservas",
                column: "CalendarioExternoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_QuartoId_UidExterno",
                table: "Reservas",
                columns: new[] { "QuartoId", "UidExterno" },
                unique: true,
                filter: "\"UidExterno\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Quartos_TokenExportacao",
                table: "Quartos",
                column: "TokenExportacao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CalendariosExternos_QuartoId",
                table: "CalendariosExternos",
                column: "QuartoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_CalendariosExternos_CalendarioExternoId",
                table: "Reservas",
                column: "CalendarioExternoId",
                principalTable: "CalendariosExternos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_CalendariosExternos_CalendarioExternoId",
                table: "Reservas");

            migrationBuilder.DropTable(
                name: "CalendariosExternos");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_CalendarioExternoId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_QuartoId_UidExterno",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Quartos_TokenExportacao",
                table: "Quartos");

            migrationBuilder.DropColumn(
                name: "CalendarioExternoId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Origem",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "SincronizadoEm",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "TituloExterno",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "UidExterno",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "TokenExportacao",
                table: "Quartos");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_QuartoId",
                table: "Reservas",
                column: "QuartoId");
        }
    }
}
