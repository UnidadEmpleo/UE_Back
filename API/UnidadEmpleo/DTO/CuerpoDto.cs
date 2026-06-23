using API.Common.Domain;
using System.ComponentModel.DataAnnotations;

namespace API.UnidadEmpleo.DTO
{
    public class CuerpoDto
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string alias { get; set; }
        public string Calle { get; set; }
        public int Numero { get; set; }
        public string Pais { get; set; }
        public int CodigoPostal { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }
        public string Colonia { get; set; }
        public List<Region> Regiones { get; set; }
    }
}
