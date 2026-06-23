using System.ComponentModel.DataAnnotations;

namespace API.Seguridad.Domain.Seguridad
{
    public class Sistema
    {
        public short Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
        [MaxLength(250)]
        public string Descripcion { get; set; }
        public bool Activo { get; set; } = true;
    }
}
