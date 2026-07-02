using API.Common.Domain;
using API.Seguridad.Domain.Audit;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace API.UnidadEmpleo.Domain
{
    public class Aspirante : IAuditable
    {
        [MaxLength(18)]
        public required string Curp { get; set; }
        [MaxLength(13)]
        public string Rfc { get; set; }
        [MaxLength(60)]
        public required string Nombre { get; set; }
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
        public string numero { get; set; }
        public string numeroInterior { get; set; }
        //public string coordenadas { get; set; }
        public float Latitud { get; set; }
        public float Longitud { get; set; }
        public string Pais { get; set; }
        public int CodigoPostal { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }
        public string Colonia { get; set; }
        public string TipoColonia { get; set; }
        public Situacion Situacion { get; set; }
        public string IdCuerpoCaptura { get; set; }
        public int IdRegionCaptura { get; set; }
        public Cuerpo CuerpoCaptura { get; set; }
        public DateTime FechaCaptura { get; set; }
        
        public List<Solicitud> Solicitudes { get; set; }

    }

    
    public enum Sexo
    {
        Femenino,
        Masculino        
    }
    public enum EdoCivil
    {
        Soltero,
        Casado,
        Divorciado,
        Viudo,
        Unión_Libre
    }
    public enum GradoEscolaridad
    {
        Primaria,
        Secundaria,
        Bachillerato,
        Licenciatura,
        Postgrado,
        Maestría,
        Doctorado
    }

    public enum EstadoEscolaridad
    {
        Concluido,
        Trunco
    }

    public enum Situacion
    {
        Captura, //Cuando se esta capturando en la region o gerencia
        Revision,//Cuando se revisa en gerencia
        Apto,    //Paso sus examenes
        No_Apto, //No paso examenes
        Dato_Transferido //Se mandan datos a nomina
    }
}
