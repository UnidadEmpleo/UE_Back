using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations.UnidadEmpleoDb
{
    /// <inheritdoc />
    public partial class solicitudBool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "fotografias",
                table: "Solicitud",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "noactaNacimiento",
                table: "Solicitud",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "nocertificadoEstudios",
                table: "Solicitud",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "nocomprobanteDomicilio",
                table: "Solicitud",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "nocurpActualizado",
                table: "Solicitud",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "noine",
                table: "Solicitud",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "nonoAntecedentesPenales",
                table: "Solicitud",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "nopre_cartillaLiberada",
                table: "Solicitud",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "norfcHomoclave",
                table: "Solicitud",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "notarjetaEnvio",
                table: "Solicitud",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "presolicitud",
                table: "Solicitud",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "referenciasDomicilio",
                table: "Solicitud",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "tarjetaEnvio",
                table: "Solicitud",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fotografias",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "noactaNacimiento",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "nocertificadoEstudios",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "nocomprobanteDomicilio",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "nocurpActualizado",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "noine",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "nonoAntecedentesPenales",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "nopre_cartillaLiberada",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "norfcHomoclave",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "notarjetaEnvio",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "presolicitud",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "referenciasDomicilio",
                table: "Solicitud");

            migrationBuilder.DropColumn(
                name: "tarjetaEnvio",
                table: "Solicitud");
        }
    }
}
