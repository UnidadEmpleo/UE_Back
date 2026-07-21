using API.UnidadEmpleo.Domain;
using System.ComponentModel.DataAnnotations;

namespace API.UnidadEmpleo.DTO
{
    public class ReferenciaDto
    {
        public string Parentesco { get; set; }

        public string ApellidoPaterno { get; set; }

        public string ApellidoMaterno { get; set; }

        public string Nombre { get; set; }

        public string Calle { get; set; }

        public string Numero { get; set; }

        public string EntreCalles { get; set; }

        public string Colonia { get; set; }

        public string CodigoPostal { get; set; }

        public string Estado { get; set; }

        public string Municipio { get; set; }

        public string Telefono{ get; set; }



//        public string numeroInterior { get; set; }
//        public string latitud { get; set; }
//        public string longitud { get; set; }
//        public string Pais { get; set; }
//        public string Colonia { get; set; }
//        public string TipoAsentamiento { get; set; }

    }
}
