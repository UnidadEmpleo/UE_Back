using API.UnidadEmpleo.Domain;
namespace API.UnidadEmpleo.DTO
{
    public class PresolicitudDto
    {

        //==================================
        // GENERALES
        //==================================

        public string NumeroPrealta { get; set; }

        public DateOnly FechaSolicitud { get; set; }

        public string Curp { get; set; }

        public string Nombre { get; set; }

        public string ApellidoPaterno { get; set; }

        public string ApellidoMaterno { get; set; }

        public DateOnly FechaNacimiento { get; set; }

        public string Sexo { get; set; }

        public string EstadoCivil { get; set; }

        public bool PensionadoISSEMYM { get; set; }

        //==================================
        // DOMICILIO
        //==================================

        public string Calle { get; set; }

        public string Numero { get; set; }

        public string NumeroInterior { get; set; }

        public string EntreCalles { get; set; }

        public string Colonia { get; set; }

        public string Municipio { get; set; }

        public string Estado { get; set; }

        public string CodigoPostal { get; set; }

        //==================================
        // TELÉFONOS
        //==================================

        public string TelefonoCasa { get; set; }

        public string TelefonoCelular { get; set; }

        public string TelefonoRecado { get; set; }

        //==================================
        // ESCOLARIDAD
        //==================================

        public string Escolaridad { get; set; }

        public bool Concluida { get; set; }

        public bool Trunca { get; set; }

        public string DocumentoEscolaridad { get; set; }

        //==================================
        // MEDIO POR EL QUE SE ENTERÓ
        //==================================

        public int MedioEntero { get; set; }

        // corpo
        public string CorporacionAlias { get; set; }

        //==================================
        // EMPLEO
        //==================================

        public bool Gobierno { get; set; }

        public bool Privada { get; set; }

        public string Empresa { get; set; }

        public string DescripcionEmpresa { get; set; }

        public string Puesto { get; set; }

        public string JefeInmediato { get; set; }

        public string TelefonoEmpresa { get; set; }

        public DateOnly FechaIngreso { get; set; }

        public DateOnly FechaSalida { get; set; }

        public string MotivoBaja { get; set; }

        //==================================
        // EXPERIENCIA
        //==================================

        public bool Policia { get; set; }

        public bool Militar { get; set; }

        public string GradoInicialPolicia { get; set; }

        public string GradoFinalPolicia { get; set; }

        public string GradoInicialMilitar { get; set; }

        public string GradoFinalMilitar { get; set; }

        //==================================
        // CORPORACIÓN
        //==================================

        public string Corporacion { get; set; }

        public string Region { get; set; }

        //==================================
        // EXPEDIENTE
        //==================================

        public bool TarjetaEnvio { get; set; }

        public bool Presolicitud { get; set; }

        public bool Fotografias { get; set; }

        public bool Croquis { get; set; }

        public bool Referencias { get; set; }

        public bool Dependientes { get; set; }

        public bool Cartilla { get; set; }

        public bool Certificado { get; set; }

        public bool ActaNacimiento { get; set; }

        public bool NoPenales { get; set; }

        public bool Comprobante { get; set; }

        public bool Cartas { get; set; }

        public bool CurpActualizada { get; set; }

        public bool Ine { get; set; }

        public bool Rfc { get; set; }

        // REFERENCIAS

        public List<Referencia> ReferenciaPersonal { get; set; }
    }
}