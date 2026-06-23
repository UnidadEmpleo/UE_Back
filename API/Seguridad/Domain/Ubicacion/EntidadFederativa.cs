using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Seguridad.Domain.Ubicacion
{
    public class EntidadFederativa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [MaxLength(10)]
        public string Abreviacion { get; set; } = string.Empty;
    }
}
