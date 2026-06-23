using Microsoft.Graph.Education.Classes.Item.Assignments.Item.Submissions.Item.Return;
using System.ComponentModel.DataAnnotations;

namespace API.Seguridad.DTOs.Seguridad
{
    public class SistemaDto
    {
        public short Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
        [MaxLength(250)]
        public string Descripcion { get; set; }
        public bool Activo { get; set; } = true;

        public string Name {
            get
            {
                return Nombre;
            }
        }
    }
}
