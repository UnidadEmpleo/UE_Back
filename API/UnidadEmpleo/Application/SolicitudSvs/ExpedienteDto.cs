using API.UnidadEmpleo.Domain;

namespace API.UnidadEmpleo.Application.SolicitudSvs
{
    public class ExpedienteDto
    {
        public int IdSoliciud { get; set; }
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
        //public Solicitud? Solicitud { get; set; }
    }
}
