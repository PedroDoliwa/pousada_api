using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPousadaIdToHospede : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PousadaId",
                table: "Hospedes",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Hospedes" h
                SET "PousadaId" = (
                    SELECT p."Id" FROM "Pousadas" p ORDER BY p."Id" LIMIT 1
                )
                WHERE EXISTS (SELECT 1 FROM "Pousadas" LIMIT 1);
                """);

            migrationBuilder.Sql("""
                DELETE FROM "Hospedes" WHERE "PousadaId" IS NULL;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "PousadaId",
                table: "Hospedes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hospedes_PousadaId",
                table: "Hospedes",
                column: "PousadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hospedes_Pousadas_PousadaId",
                table: "Hospedes",
                column: "PousadaId",
                principalTable: "Pousadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hospedes_Pousadas_PousadaId",
                table: "Hospedes");

            migrationBuilder.DropIndex(
                name: "IX_Hospedes_PousadaId",
                table: "Hospedes");

            migrationBuilder.DropColumn(
                name: "PousadaId",
                table: "Hospedes");
        }
    }
}
