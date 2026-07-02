using API.Common.Domain;
using API.Seguridad.Domain.Audit;
using System.ComponentModel.DataAnnotations;

namespace API.UnidadEmpleo.Domain
{
    public class Solicitud : IAuditable
    {
        public int Id { get; set; }
        public DateOnly FechaSolicitud { get; set; }
        public Boolean StatusExp { get; set; } // true = completo, false = incompleto
        public Boolean Revalorable { get; set; }
        public StatusSolicitud Status { get; set; }
        public string Observaciones { get; set; }
        public string CorporacionId { get; set; }
        public int RegionId { get; set; }
        public string Curp { get; set; }
        
        public Cuerpo Corporacion { get; set; }
        public Region Region { get; set; }
        public Aspirante Aspirante { get; set; }
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

        public List<Referencia> Referencias { get; set; }
        public List<Evaluacion> Evaluaciones { get; set; }
        public List<CartaCompromiso> CartasCompromiso { get; set; }

        public DateTime? FechaUltimaActualizacion { get ; set;  }

    }

    public class Referencia : IAuditable
    {
        public int Id {  get; set; }
        public int IdSoliciud { get; set; }
        public Parentesco Parentesco { get; set; }
        public string TelefonoLocal { get; set; }

        [MaxLength(60)]
        public required string Nombre { get; set; }
        [MaxLength(60)]
        public string Apellido_Paterno { get; set; }
        [MaxLength(60)]
        public string Apellido_Materno { get; set; }
        public string Calle { get; set; }
        public string numero { get; set; }
        public string numeroInterior { get; set; }
        public string EntreCalles { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string Pais { get; set; }
        public int CodigoPostal { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }
        public string Colonia { get; set; }
        public string TipoAsentamiento { get; set; }

        public Solicitud Solicitud { get; set; }
    }

    public class CartaCompromiso : IAuditable
    {
        public int Id { get; set; }
        public int IdSoliciud { get; set; }
        public TipoCarta tipo {  get; set; }
        public StatusDocumento Status {  get; set; }
        public DateOnly FechaEmision {  get; set; }
        public DateOnly FechaCompromiso { get; set; }
        public Solicitud Solicitud { get; set; }
    }

    public class Evaluacion : IAuditable
    {
        public int Id { get; set; }
        public DateTime Ingreso{ get; set; }
        public DateTime? Salida { get; set; }
        public Boolean Resultado { get; set; }
        public string Observaciones { get; set; }
        public Boolean revalorable { get; set; }
        public int IdSoliciud { get; set; }
        public TipoEvaluacion TipoEvaluacion { get; set; }
        public Solicitud Solicitud { get; set; }
        public string UsuarioSalida{ get; set; }
        public string UsuarioIngreso { get; set; }
        public string UsuarioEvaluo { get; set; }
        public string NombreUsuarioEvaluo { get; set; }

    }
    
    
    // ENUM
    
    public enum TipoEvaluacion
    {
        Registro = 1,
        Medico = 2,
        Psicologico = 3,
        Toxicologico = 4,
        PIE = 5
    }

    public enum EnteraEmpleo : int
    {
        Reingreso = 1,
        Volante = 2,
        Reclutador = 3,
        Empresa_Particular = 4,
        Centro_de_Reclutamiento = 5,
        Iniciativa_Propia = 6,
        Feria_Empleo = 7,
        Bolsa_Trabajo = 8,
        Conocido_Familiar_Corporacion=9
    }

    public enum StatusDocumento
    {
        Entregado,
        Pendiente,
        Pendiente_vencer,
        Extemporaneo
    }

    public enum TipoCarta
    {
        Cartilla,
        Certificado_Estudios,
        Peso, 
        Tatuajes,
        Antecedentes_Penales
    }
    
    public enum StatusSolicitud
    {
        Captura = 1,
        Evaluando = 2,
        No_apto = 3,
        Apto = 4
    }
    

    public enum Parentesco
    {
        Padre=1,
        Madre=2,
        Esposa=3,
        Hermano=4,
        Tio=5,
        Primo=6,
        Sobrino=7,
        Cuñado=8,
        Abuelo=9,
        Suegro=10,
        Amigo=11,
        Vecino=12,
        Conocido=13
    }


}
