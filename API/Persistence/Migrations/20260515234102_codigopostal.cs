using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class codigopostal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CodigoPostal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    c_mnpio = table.Column<int>(type: "int", nullable: false),
                    c_estado = table.Column<int>(type: "int", nullable: false),
                    c_tipo_asenta = table.Column<int>(type: "int", nullable: false),
                    c_codigo = table.Column<int>(type: "int", nullable: false),
                    d_mnpio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    d_estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    d_tipo_asenta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    d_asenta = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodigoPostal", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodigoPostal");
        }
    }
}
