using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class nuevo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CuerpoId",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CuerpoId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Usuarios");
        }
    }
}
