using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations.UnidadEmpleoDb
{
    /// <inheritdoc />
    public partial class addlatlngAspirante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "coordenadas",
                table: "Aspirante",
                newName: "TipoColonia");

            migrationBuilder.AddColumn<int>(
                name: "enteraEmpleo",
                table: "Solicitud",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Latitud",
                table: "Aspirante",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Longitud",
                table: "Aspirante",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "numeroInterior",
                table: "Aspirante",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "enteraEmpleo",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "Latitud",
                table: "Aspirante");

            migrationBuilder.DropColumn(
                name: "Longitud",
                table: "Aspirante");

            migrationBuilder.DropColumn(
                name: "numeroInterior",
                table: "Aspirante");

            migrationBuilder.RenameColumn(
                name: "TipoColonia",
                table: "Aspirante",
                newName: "coordenadas");
        }
    }
}
