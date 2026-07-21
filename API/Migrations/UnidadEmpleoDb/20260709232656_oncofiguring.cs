using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations.UnidadEmpleoDb
{
    /// <inheritdoc />
    public partial class oncofiguring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Anexos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LinkId = table.Column<int>(type: "int", nullable: false),
                    TipoLink = table.Column<int>(type: "int", nullable: false),
                    TipoAnexo = table.Column<int>(type: "int", nullable: false),
                    TipoArchivo = table.Column<int>(type: "int", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Blob = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anexos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Corporacion",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    alias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Calle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numero = table.Column<int>(type: "int", maxLength: 120, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoPostal = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Municipio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Colonia = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corporacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityChangeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeType = table.Column<int>(type: "int", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Changes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityChangeLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    region = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Aspirante",
                columns: table => new
                {
                    Curp = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    Rfc = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Apellido_Paterno = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Apellido_Materno = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Fecha_Nacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    Sexo = table.Column<int>(type: "int", nullable: false),
                    TelefonoCelular = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Foto = table.Column<int>(type: "int", nullable: true),
                    Estado_Civil = table.Column<int>(type: "int", nullable: false),
                    Grado_Escolaridad = table.Column<int>(type: "int", nullable: false),
                    EscolaridadConcluidaTrunca = table.Column<int>(type: "int", nullable: false),
                    DocumentoAcreditaEscolaridad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PensionaodISSEMYM = table.Column<bool>(type: "bit", nullable: false),
                    Calle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntreCalles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numeroInterior = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitud = table.Column<float>(type: "real", nullable: false),
                    Longitud = table.Column<float>(type: "real", nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoPostal = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Municipio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Colonia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoColonia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Situacion = table.Column<int>(type: "int", nullable: false),
                    IdCuerpoCaptura = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdRegionCaptura = table.Column<int>(type: "int", nullable: false),
                    FechaCaptura = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aspirante", x => x.Curp);
                    table.ForeignKey(
                        name: "FK_Aspirante_Corporacion_IdCuerpoCaptura",
                        column: x => x.IdCuerpoCaptura,
                        principalTable: "Corporacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CuerpoRegion",
                columns: table => new
                {
                    CuerposId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RegionesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuerpoRegion", x => new { x.CuerposId, x.RegionesId });
                    table.ForeignKey(
                        name: "FK_CuerpoRegion_Corporacion_CuerposId",
                        column: x => x.CuerposId,
                        principalTable: "Corporacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CuerpoRegion_Region_RegionesId",
                        column: x => x.RegionesId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Solicitud",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaSolicitud = table.Column<DateOnly>(type: "date", nullable: false),
                    StatusExp = table.Column<bool>(type: "bit", nullable: false),
                    Revalorable = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorporacionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    Curp = table.Column<string>(type: "nvarchar(18)", nullable: false),
                    TelefonoCasa = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TelefonoRecado = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    enteraEmpleo = table.Column<int>(type: "int", nullable: false),
                    Gobierno = table.Column<bool>(type: "bit", nullable: false),
                    Privada = table.Column<bool>(type: "bit", nullable: false),
                    NombreEmpresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescripcionEmpresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Puesto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JefeInmediato = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelefonoEmpleo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaFinal = table.Column<DateOnly>(type: "date", nullable: false),
                    MotivoBaja = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Policia = table.Column<bool>(type: "bit", nullable: false),
                    GradoInicioPolicia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GradoFinalPolicia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Militar = table.Column<bool>(type: "bit", nullable: false),
                    GradoInicioMilitar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GradoFinalMilitar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fotos = table.Column<bool>(type: "bit", nullable: false),
                    coordenadasVivienda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Croquis = table.Column<bool>(type: "bit", nullable: false),
                    DependienteEconomico = table.Column<bool>(type: "bit", nullable: false),
                    CartillaLiberada = table.Column<bool>(type: "bit", nullable: false),
                    CertificadoEstudios = table.Column<bool>(type: "bit", nullable: false),
                    ActaNacimiento = table.Column<bool>(type: "bit", nullable: false),
                    NoAntecedentesPenales = table.Column<bool>(type: "bit", nullable: false),
                    ComprobanteDomicilio = table.Column<bool>(type: "bit", nullable: false),
                    CartasRecomendacion = table.Column<bool>(type: "bit", nullable: false),
                    CurpActualizado = table.Column<bool>(type: "bit", nullable: false),
                    Ine = table.Column<bool>(type: "bit", nullable: false),
                    RfcHomoclave = table.Column<bool>(type: "bit", nullable: false),
                    tarjetaEnvio = table.Column<bool>(type: "bit", nullable: false),
                    presolicitud = table.Column<bool>(type: "bit", nullable: false),
                    fotografias = table.Column<bool>(type: "bit", nullable: false),
                    referenciasDomicilio = table.Column<bool>(type: "bit", nullable: false),
                    notarjetaEnvio = table.Column<int>(type: "int", nullable: false),
                    nopre_cartillaLiberada = table.Column<int>(type: "int", nullable: false),
                    nocertificadoEstudios = table.Column<int>(type: "int", nullable: false),
                    noactaNacimiento = table.Column<int>(type: "int", nullable: false),
                    nonoAntecedentesPenales = table.Column<int>(type: "int", nullable: false),
                    nocomprobanteDomicilio = table.Column<int>(type: "int", nullable: false),
                    nocurpActualizado = table.Column<int>(type: "int", nullable: false),
                    noine = table.Column<int>(type: "int", nullable: false),
                    norfcHomoclave = table.Column<int>(type: "int", nullable: false),
                    FechaUltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitud", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solicitud_Aspirante_Curp",
                        column: x => x.Curp,
                        principalTable: "Aspirante",
                        principalColumn: "Curp",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Solicitud_Corporacion_CorporacionId",
                        column: x => x.CorporacionId,
                        principalTable: "Corporacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Solicitud_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartaCompromiso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSoliciud = table.Column<int>(type: "int", nullable: false),
                    tipo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FechaEmision = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaCompromiso = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartaCompromiso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartaCompromiso_Solicitud_IdSoliciud",
                        column: x => x.IdSoliciud,
                        principalTable: "Solicitud",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evaluacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ingreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Salida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Resultado = table.Column<bool>(type: "bit", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    revalorable = table.Column<bool>(type: "bit", nullable: false),
                    IdSoliciud = table.Column<int>(type: "int", nullable: false),
                    TipoEvaluacion = table.Column<int>(type: "int", nullable: false),
                    UsuarioSalida = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioIngreso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioEvaluo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreUsuarioEvaluo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluacion_Solicitud_IdSoliciud",
                        column: x => x.IdSoliciud,
                        principalTable: "Solicitud",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Referencia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSoliciud = table.Column<int>(type: "int", nullable: false),
                    Parentesco = table.Column<int>(type: "int", nullable: false),
                    TelefonoLocal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Apellido_Paterno = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Apellido_Materno = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Calle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numeroInterior = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntreCalles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    latitud = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    longitud = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoPostal = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Municipio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Colonia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoAsentamiento = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Referencia_Solicitud_IdSoliciud",
                        column: x => x.IdSoliciud,
                        principalTable: "Solicitud",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aspirante_IdCuerpoCaptura",
                table: "Aspirante",
                column: "IdCuerpoCaptura");

            migrationBuilder.CreateIndex(
                name: "IX_CartaCompromiso_IdSoliciud",
                table: "CartaCompromiso",
                column: "IdSoliciud");

            migrationBuilder.CreateIndex(
                name: "IX_CuerpoRegion_RegionesId",
                table: "CuerpoRegion",
                column: "RegionesId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluacion_IdSoliciud",
                table: "Evaluacion",
                column: "IdSoliciud");

            migrationBuilder.CreateIndex(
                name: "IX_Referencia_IdSoliciud",
                table: "Referencia",
                column: "IdSoliciud");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitud_CorporacionId",
                table: "Solicitud",
                column: "CorporacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitud_Curp",
                table: "Solicitud",
                column: "Curp");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitud_RegionId",
                table: "Solicitud",
                column: "RegionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anexos");

            migrationBuilder.DropTable(
                name: "CartaCompromiso");

            migrationBuilder.DropTable(
                name: "CuerpoRegion");

            migrationBuilder.DropTable(
                name: "EntityChangeLogs");

            migrationBuilder.DropTable(
                name: "Evaluacion");

            migrationBuilder.DropTable(
                name: "Referencia");

            migrationBuilder.DropTable(
                name: "Solicitud");

            migrationBuilder.DropTable(
                name: "Aspirante");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropTable(
                name: "Corporacion");
        }
    }
}
