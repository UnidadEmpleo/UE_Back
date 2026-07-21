using API.Common.Domain;
using API.UnidadEmpleo.Domain;
using System.ComponentModel.DataAnnotations;

namespace API.UnidadEmpleo.DTO
{
    public class SolicitudDto
    {
        public int Id { get; set; }
        public DateOnly FechaSolicitud { get; set; }
        public Boolean StatusExp { get; set; }
        public Boolean Revalorable { get; set; }
        public StatusSolicitud Status { get; set; }
        public string Observaciones { get; set; }
        public string CorporacionId { get; set; }
        public int RegionId { get; set; }
        public string Curp { get; set; }

        public Cuerpo Corporacion { get; set; }
        public Region Region { get; set; }
        public AspiranteDtoBasic Aspirante { get; set; }
        [MaxLength(10)]
        public string TelefonoCasa { get; set; }
        [MaxLength(10)]
        public string TelefonoRecado { get; set; }
        public EnteraEmpleo enteraEmpleo { get; set; }
        //ULTIMO EMPLEO
        public Boolean Gobierno { get; set; }
        public Boolean Privada { get; set; }
        public string NombreEmpresa { get; set; }
        public string DescripcionEmpresa { get; set; }
        public string Puesto { get; set; }
        public string JefeInmediato { get; set; }
        public string TelefonoEmpleo { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFinal { get; set; }
        public string MotivoBaja { get; set; }

        public Boolean Policia { get; set; }
        public string GradoInicioPolicia { get; set; }
        public string GradoFinalPolicia { get; set; }


        public Boolean Militar { get; set; }
        public string GradoInicioMilitar { get; set; }
        public string GradoFinalMilitar { get; set; }

        //EXPEDIENTE
        public Boolean Fotos { get; set; }
        public string coordenadasVivienda { get; set; }
        public Boolean Croquis { get; set; }
        public Boolean DependienteEconomico { get; set; }
        public Boolean CartillaLiberada { get; set; }
        public Boolean CertificadoEstudios { get; set; }
        public Boolean ActaNacimiento { get; set; }
        public Boolean NoAntecedentesPenales { get; set; }
        public Boolean ComprobanteDomicilio { get; set; }
        public Boolean CartasRecomendacion { get; set; }
        public Boolean CurpActualizado { get; set; }
        public Boolean Ine { get; set; }
        public Boolean RfcHomoclave { get; set; }

        public Boolean tarjetaEnvio { get; set; }
        public Boolean presolicitud { get; set; }
        public Boolean fotografias { get; set; }
        public Boolean referenciasDomicilio { get; set; }

        public int notarjetaEnvio { get; set; }
        public int nopre_cartillaLiberada { get; set; }
        public int nocertificadoEstudios { get; set; }
        public int noactaNacimiento { get; set; }
        public int nonoAntecedentesPenales { get; set; }
        public int nocomprobanteDomicilio { get; set; }
        public int nocurpActualizado { get; set; }
        public int noine { get; set; }
        public int norfcHomoclave { get; set; }

        
        




        /*
        public List<Referencia> Referencias { get; set; }
        public List<Evaluacion> Evaluaciones { get; set; }
        public List<CartaCompromiso> CartasCompromiso { get; set; }
        */
    }
}
