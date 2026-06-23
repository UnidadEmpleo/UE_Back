using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations.UnidadEmpleoDb
{
    /// <inheritdoc />
    public partial class situacioncapturaaspirante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdCuerpoCaptura",
                table: "Aspirante",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IdRegionCaptura",
                table: "Aspirante",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Situacion",
                table: "Aspirante",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Aspirante_IdCuerpoCaptura",
                table: "Aspirante",
                column: "IdCuerpoCaptura");

            migrationBuilder.AddForeignKey(
                name: "FK_Aspirante_Corporacion_IdCuerpoCaptura",
                table: "Aspirante",
                column: "IdCuerpoCaptura",
                principalTable: "Corporacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aspirante_Corporacion_IdCuerpoCaptura",
                table: "Aspirante");

            migrationBuilder.DropIndex(
                name: "IX_Aspirante_IdCuerpoCaptura",
                table: "Aspirante");

            migrationBuilder.DropColumn(
                name: "IdCuerpoCaptura",
                table: "Aspirante");

            migrationBuilder.DropColumn(
                name: "IdRegionCaptura",
                table: "Aspirante");

            migrationBuilder.DropColumn(
                name: "Situacion",
                table: "Aspirante");
        }
    }
}
