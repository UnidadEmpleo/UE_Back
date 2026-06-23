using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConexionBDSistemaCorporacion_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "SistemaId",
                table: "Procesos",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "BasesDatos",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ServerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatabaseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Port = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasesDatos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sistemas",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sistemas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CorporacionSistemaBDs",
                columns: table => new
                {
                    CorporacionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SistemaId = table.Column<short>(type: "smallint", nullable: false),
                    BaseDatosId = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporacionSistemaBDs", x => new { x.CorporacionId, x.SistemaId, x.BaseDatosId });
                    table.ForeignKey(
                        name: "FK_CorporacionSistemaBDs_BasesDatos_BaseDatosId",
                        column: x => x.BaseDatosId,
                        principalTable: "BasesDatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorporacionSistemaBDs_Corporaciones_CorporacionId",
                        column: x => x.CorporacionId,
                        principalTable: "Corporaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorporacionSistemaBDs_Sistemas_SistemaId",
                        column: x => x.SistemaId,
                        principalTable: "Sistemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporacionSistemaBDs_BaseDatosId",
                table: "CorporacionSistemaBDs",
                column: "BaseDatosId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporacionSistemaBDs_SistemaId",
                table: "CorporacionSistemaBDs",
                column: "SistemaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporacionSistemaBDs");

            migrationBuilder.DropTable(
                name: "BasesDatos");

            migrationBuilder.DropTable(
                name: "Sistemas");

            migrationBuilder.DropColumn(
                name: "SistemaId",
                table: "Procesos");
        }
    }
}
