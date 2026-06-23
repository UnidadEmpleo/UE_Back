using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations.UnidadEmpleoDb
{
    /// <inheritdoc />
    public partial class cuerpoalias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "alias",
                table: "Corporacion",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "alias",
                table: "Corporacion");
        }
    }
}
