using API.UnidadEmpleo.Domain;

namespace API.Seguridad.DTOs.Anexos
{
    public class AnexoDTO
    {
        public int Id { get; set; }
        public int LinkId { get; set; }
        public TiposLink TipoLink { get; set; } = TiposLink.Otro;
        public TiposAnexo TipoAnexo { get; set; } = TiposAnexo.Otro;
        public TiposArchivo TipoArchivo { get; set; } = TiposArchivo.Otro;
        public string? NombreArchivo { get; set; }
        public string? Extension { get; set; }
        public string? BlobBase64 { get; set; }
    }
}