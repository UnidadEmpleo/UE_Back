using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class agregafkprocesosistema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Procesos_SistemaId",
                table: "Procesos",
                column: "SistemaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Procesos_Sistemas_SistemaId",
                table: "Procesos",
                column: "SistemaId",
                principalTable: "Sistemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Procesos_Sistemas_SistemaId",
                table: "Procesos");

            migrationBuilder.DropIndex(
                name: "IX_Procesos_SistemaId",
                table: "Procesos");
        }
    }
}
