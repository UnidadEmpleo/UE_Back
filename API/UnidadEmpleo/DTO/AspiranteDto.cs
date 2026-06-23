using API.UnidadEmpleo.Domain;
using System.ComponentModel.DataAnnotations;

namespace API.UnidadEmpleo.DTO
{
    public class AspiranteDto
    {
        [MaxLength(18)]
        public required string Curp { get; set; }
        [MaxLength(13)]
        public string Rfc { get; set; }
        [MaxLength(60)]
        public required string Nombre { get; set; } = "";
        [MaxLength(60)]
        public string Apellido_Paterno { get; set; }
        [MaxLength(60)]
        public string Apellido_Materno { get; set; }
        public required DateOnly Fecha_Nacimiento { get; set; }
        public Sexo Sexo { get; set; }
        public string TelefonoCelular { get; set; }
        public int? Foto { get; set; }
        public EdoCivil Estado_Civil { get; set; }
        public GradoEscolaridad Grado_Escolaridad { get; set; }
        public EstadoEscolaridad EscolaridadConcluidaTrunca { get; set; }
        public string DocumentoAcreditaEscolaridad { get; set; }
        public Boolean PensionaodISSEMYM { get; set; }

        public string Calle { get; set; }
        public string EntreCalles { get; set; }
        public int numero { get; set; }
        public string coordenadas { get; set; }
        public string Pais { get; set; }
        public int CodigoPostal { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }
        public string Colonia { get; set; }
        public Situacion Situacion { get; set; }
        public string IdCuerpoCaptura { get; set; }
        public int IdRegionCaptura { get; set; }
        public RegioDto Region { get; set; }
        public CuerpoDto Cuerpo { get; set; }
    }

    public class AspiranteDtoBasic
    {
        public required string Curp { get; set; }
        
        public string Rfc { get; set; }
        
        public required string Nombre { get; set; } = "";
        
        public string Apellido_Paterno { get; set; }
        
        public string Apellido_Materno { get; set; }
        public required DateOnly Fecha_Nacimiento { get; set; }
        public Sexo Sexo { get; set; }
    }
}
