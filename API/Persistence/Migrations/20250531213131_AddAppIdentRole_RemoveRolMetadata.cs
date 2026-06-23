using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddAppIdentRole_RemoveRolMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppRoleMetadatas");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedAt",
                table: "AspNetUsers",
                newName: "FechaUltimaActualizacion");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "AspNetUsers",
                newName: "FechaCreacion");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "AspNetRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaActualizacion",
                table: "AspNetRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "FechaUltimaActualizacion",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "AspNetRoles");

            migrationBuilder.RenameColumn(
                name: "FechaUltimaActualizacion",
                table: "AspNetUsers",
                newName: "LastUpdatedAt");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "AspNetUsers",
                newName: "CreatedAt");

            migrationBuilder.CreateTable(
                name: "AppRoleMetadatas",
                columns: table => new
                {
                    AppRoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaUltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRoleMetadatas", x => x.AppRoleId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppRoleMetadatas_Value",
                table: "AppRoleMetadatas",
                column: "Value",
                unique: true);
        }
    }
}
