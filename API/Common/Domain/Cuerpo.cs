using API.Seguridad.Domain.Audit;
using System.ComponentModel.DataAnnotations;

namespace API.Common.Domain
{
    public class Cuerpo : IAuditable
    {
        public string Id { get; set; }

        [MaxLength(120)]
        public string Nombre { get; set; }
        public string alias { get; set; }
        public string Calle { get; set; }
        [MaxLength(120)]
        public int Numero { get; set; }
        public string Pais { get; set; }
        public int CodigoPostal { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }
        public string Colonia { get; set; }
        public List<Region> Regiones { get; set; }

    }
}
