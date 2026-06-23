namespace API.Seguridad.Domain.Seguridad
{
    public class CorporacionSistemaBD
    {
        public string CorporacionId { get; set; }
        public Corporacion Corporacion { get; set; }
        public short SistemaId { get; set; }
        public Sistema Sistema { get; set; }
        public short BaseDatosId { get; set; }
        public BaseDatos BaseDatos { get; set; }
    }
}
