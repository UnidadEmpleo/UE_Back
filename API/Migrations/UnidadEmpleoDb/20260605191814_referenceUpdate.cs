using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations.UnidadEmpleoDb
{
    /// <inheritdoc />
    public partial class referenceUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "coordenadas",
                table: "Referencia",
                newName: "numeroInterior");

            migrationBuilder.AlterColumn<string>(
                name: "numero",
                table: "Referencia",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "TipoAsentamiento",
                table: "Referencia",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "latitud",
                table: "Referencia",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "longitud",
                table: "Referencia",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoAsentamiento",
                table: "Referencia");

            migrationBuilder.DropColumn(
                name: "latitud",
                table: "Referencia");

            migrationBuilder.DropColumn(
                name: "longitud",
                table: "Referencia");

            migrationBuilder.RenameColumn(
                name: "numeroInterior",
                table: "Referencia",
                newName: "coordenadas");

            migrationBuilder.AlterColumn<int>(
                name: "numero",
                table: "Referencia",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
