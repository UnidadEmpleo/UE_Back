using API.Seguridad.Application.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.UnidadEmpleo.Domain
{
    /// <summary>
    /// Entidad para almacenar anexos multimedia o archivos relacionados a pacientes o usuarios.
    /// </summary>
    public class Anexo
    {
        public int Id { get; set; }
        public int LinkId { get; set; }
        public TiposLink TipoLink { get; set; }
        public TiposAnexo TipoAnexo { get; set; }
        public TiposArchivo TipoArchivo { get; set; } = TiposArchivo.Otro;

        [MaxLength(255)]
        public string? NombreArchivo { get; set; }

        [MaxLength(5)]
        public string? Extension
        {
            get
            {
                if (NombreArchivo.IsNullOrWhiteSpace())
                    return string.Empty;
                return Path.GetExtension(NombreArchivo).Replace(".", "").ToUpper();
            }
            set { }
        }

        public byte[] Blob { get; set; } = Array.Empty<byte>();
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Enumerador para el tipo de relación del anexo.
    /// </summary>
    public enum TiposLink
    {
        Otro,
        Paciente,
        Usuario
    }

    /// <summary>
    /// Enumerador para el tipo de anexo.
    /// </summary>
    public enum TiposAnexo
    {
        Otro,
        Foto,
        Archivo
    }

    public enum TiposArchivo
    {
        Otro,
        PDF,
        Word,
        Excel,
        Imagen,
        Video,
        Audio,
        PPT,
        Texto,
        Comprimido
    }
}