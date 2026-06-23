using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BorrarColumnas_Corporacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatabaseName",
                table: "Corporaciones");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Corporaciones");

            migrationBuilder.DropColumn(
                name: "Port",
                table: "Corporaciones");

            migrationBuilder.DropColumn(
                name: "ServerName",
                table: "Corporaciones");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Corporaciones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DatabaseName",
                table: "Corporaciones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Corporaciones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Port",
                table: "Corporaciones",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "1433");

            migrationBuilder.AddColumn<string>(
                name: "ServerName",
                table: "Corporaciones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Corporaciones",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
