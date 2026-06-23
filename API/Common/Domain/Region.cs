using System.ComponentModel.DataAnnotations;

namespace API.Common.Domain
{
    public class Region
    {
        public int Id { get; set; }
        
        [MaxLength(120)]
        public string region { get; set; }
        public List<Cuerpo> Cuerpos { get; set; }

    }
}
