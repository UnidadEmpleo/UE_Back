using API.Seguridad.Application.Core;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.DTO;
using API.UnidadEmpleo.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.UnidadEmpleo.Application.AspiranteApp;

public class GetPresolicitud
{
    public class Query : IRequest<Result<PresolicitudDto>>
    {
        public int IdSolicitud { get; set; }
        
    }

    public class Handler : IRequestHandler<Query, Result<PresolicitudDto>>
    {
        private readonly UnidadEmpleoDbContext _dbContext;

        public Handler(UnidadEmpleoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<PresolicitudDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            
            IQueryable<Solicitud> consulta = _dbContext.Set<Solicitud>()
                .Include(x => x.Aspirante)
                .Include(x => x.Region)
                .Include(x => x.Corporacion)
                .Include(x => x.Referencias)
                .AsNoTracking();

             
            consulta = consulta.Where(x => x.Id == request.IdSolicitud);

            var s = await consulta.FirstOrDefaultAsync(cancellationToken);

            if (s == null)
                return Result<PresolicitudDto>.Failure(
                    "No se encontró la solicitud.",
                    404);

            var dto = new PresolicitudDto
            {
                // REFERENCIAS

                ReferenciaPersonal = s.Referencias,

                //---------------------------------------
                // GENERALES
                //---------------------------------------

                NumeroPrealta = s.Id.ToString(),

                FechaSolicitud = s.FechaSolicitud,

                Curp = s.Aspirante.Curp,

                Nombre = s.Aspirante.Nombre,

                ApellidoPaterno = s.Aspirante.Apellido_Paterno,

                ApellidoMaterno = s.Aspirante.Apellido_Materno,

                FechaNacimiento = s.Aspirante.Fecha_Nacimiento,

                Sexo = s.Aspirante.Sexo.ToString(),

                EstadoCivil = s.Aspirante.Estado_Civil.ToString(),

                PensionadoISSEMYM = s.Aspirante.PensionaodISSEMYM,

                //---------------------------------------
                // DOMICILIO
                //---------------------------------------

                Calle = s.Aspirante.Calle,

                Numero = s.Aspirante.numero,

                NumeroInterior = s.Aspirante.numeroInterior,

                EntreCalles = s.Aspirante.EntreCalles,

                Colonia = s.Aspirante.Colonia,

                Municipio = s.Aspirante.Municipio,

                Estado = s.Aspirante.Estado,

                CodigoPostal = s.Aspirante.CodigoPostal.ToString(),

                //---------------------------------------
                // TELÉFONOS
                //---------------------------------------

                TelefonoCasa = s.TelefonoCasa,

                TelefonoCelular = s.Aspirante.TelefonoCelular,

                TelefonoRecado = s.TelefonoRecado,

                //---------------------------------------
                // ESCOLARIDAD
                //---------------------------------------

                Escolaridad = s.Aspirante.Grado_Escolaridad.ToString(),

                DocumentoEscolaridad = s.Aspirante.DocumentoAcreditaEscolaridad,

                Concluida =
                    s.Aspirante.EscolaridadConcluidaTrunca ==
                    EstadoEscolaridad.Concluido,

                Trunca =
                    s.Aspirante.EscolaridadConcluidaTrunca ==
                    EstadoEscolaridad.Trunco,

                //---------------------------------------
                // MEDIO ENTERO
                //---------------------------------------

                MedioEntero = (int)s.enteraEmpleo,

                // corpor

                CorporacionAlias = s.Corporacion.alias,

                //---------------------------------------
                // EMPLEO
                //---------------------------------------

                Gobierno = s.Gobierno,

                Privada = s.Privada,

                Empresa = s.NombreEmpresa,

                Puesto = s.Puesto,

                DescripcionEmpresa = s.DescripcionEmpresa,

                JefeInmediato = s.JefeInmediato,

                TelefonoEmpresa = s.TelefonoEmpleo,

                FechaIngreso = s.FechaInicio,

                FechaSalida = s.FechaFinal,

                MotivoBaja = s.MotivoBaja,

                //---------------------------------------
                // EXPERIENCIA
                //---------------------------------------

                Policia = s.Policia,

                Militar = s.Militar,

                GradoInicialPolicia = s.GradoInicioPolicia,

                GradoFinalPolicia = s.GradoFinalPolicia,

                GradoInicialMilitar = s.GradoInicioMilitar,

                GradoFinalMilitar = s.GradoFinalMilitar,

                //---------------------------------------
                // CORPORACIÓN
                //---------------------------------------

                Corporacion = s.CorporacionId,

                Region = s.RegionId.ToString(),

                //---------------------------------------
                // EXPEDIENTE
                //---------------------------------------

                TarjetaEnvio = s.tarjetaEnvio,

                Presolicitud = s.presolicitud,

                Fotografias = s.fotografias,

                Croquis = s.Croquis,

                Referencias = s.referenciasDomicilio,

                Dependientes = s.DependienteEconomico,

                Cartilla = s.CartillaLiberada,

                Certificado = s.CertificadoEstudios,

                ActaNacimiento = s.ActaNacimiento,

                NoPenales = s.NoAntecedentesPenales,

                Comprobante = s.ComprobanteDomicilio,

                Cartas = s.CartasRecomendacion,

                CurpActualizada = s.CurpActualizado,

                Ine = s.Ine,

                Rfc = s.RfcHomoclave

            };

            return Result<PresolicitudDto>.Success(dto);
        }
    }
}