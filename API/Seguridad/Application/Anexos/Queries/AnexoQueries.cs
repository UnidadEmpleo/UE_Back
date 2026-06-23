
using API.Seguridad.Application.Core;
using API.Seguridad.DTOs.Anexos;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Seguridad.Application.Anexos.Queries
{
    public static class GetAnexoById
    {
        public class Query : IRequest<Result<AnexoDTO>>
        {
            public int Id { get; set; }
        }
    }

    public static class GetAnexosByPacienteId
    {
        public class Query : IRequest<Result<List<AnexoDTO>>>
        {
            public int PacienteId { get; set; }
        }
    }

    public static class GetAnexoBlobById
    {
        public class Query : IRequest<Result<string>>
        {
            public int Id { get; set; }
        }

        public class Handler(
            UnidadEmpleoDBContextFactoryInterface factory
        ) : IRequestHandler<Query, Result<string>>
        {
            public async Task<Result<string>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var db = await factory.CreateAsync();
                var anexo = await db.Anexos
                    .AsNoTracking()
                    .Where(x => x.Id == request.Id)
                    .Select(x => x.Blob)
                    .FirstOrDefaultAsync(cancellationToken);

                if (anexo == null)
                    return Result<string>.Failure("Contenido no encontrado", 404);

                if (anexo.Length <= 0)
                    return Result<string>.Failure("Anexo sin contenido", 404);

                return Result<string>.Success(Convert.ToBase64String(anexo));
            }
        }
    }

    public class GetAnexoByIdHandler(
        UnidadEmpleoDBContextFactoryInterface factory,
        IMapper mapper
    ) : IRequestHandler<GetAnexoById.Query, Result<AnexoDTO>>
    {
        public async Task<Result<AnexoDTO>> Handle(GetAnexoById.Query request, CancellationToken cancellationToken)
        {
            await using var db = await factory.CreateAsync();
            var anexo = await db.Anexos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (anexo == null)
                return Result<AnexoDTO>.Failure("Anexo no encontrado", 404);

            var dto = mapper.Map<AnexoDTO>(anexo);
            return Result<AnexoDTO>.Success(dto);
        }
    }

    public class GetAnexosByPacienteIdHandler(
        UnidadEmpleoDBContextFactoryInterface factory,
        IMapper mapper
    ) : IRequestHandler<GetAnexosByPacienteId.Query, Result<List<AnexoDTO>>>
    {
        public async Task<Result<List<AnexoDTO>>> Handle(GetAnexosByPacienteId.Query request, CancellationToken cancellationToken)
        {
            await using var db = await factory.CreateAsync();
            var anexos = await db.Anexos
                .AsNoTracking()
                .Where(x => x.LinkId == request.PacienteId)
                .Select(x => new AnexoDTO
                {
                    Id = x.Id,
                    LinkId = x.LinkId,
                    TipoLink = x.TipoLink,
                    TipoAnexo = x.TipoAnexo,
                    TipoArchivo = x.TipoArchivo,
                    NombreArchivo = x.NombreArchivo,
                    Extension = x.Extension
                })
                .ToListAsync(cancellationToken);

            return Result<List<AnexoDTO>>.Success(anexos);
        }
    }

    public static class GetAnexoFileById
    {
        public class Query : IRequest<Result<(byte[] Blob, string FileName, string MimeType)>>
        {
            public int Id { get; set; }
        }

        public class Handler(
            UnidadEmpleoDBContextFactoryInterface factory
        ) : IRequestHandler<Query, Result<(byte[] Blob, string FileName, string MimeType)>>
        {
            public async Task<Result<(byte[] Blob, string FileName, string MimeType)>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var db = await factory.CreateAsync();
                var anexo = await db.Anexos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (anexo == null || anexo.Blob == null || anexo.Blob.Length == 0)
                    return Result<(byte[], string, string)>.Failure("Archivo no encontrado", 404);

                var mimeType = GetMimeType(anexo.Extension);
                
                var fileName = anexo.NombreArchivo ?? "file";
                if (anexo.Extension != null && !fileName.EndsWith(anexo.Extension, StringComparison.OrdinalIgnoreCase))
                {
                    fileName += anexo.Extension;
                }
                
                return Result<(byte[], string, string)>.Success((anexo.Blob, fileName, mimeType));
            }

            private static string GetMimeType(string? extension)
            {
                return extension?.ToLower() switch
                {
                    ".pdf"  => "application/pdf",
                    ".jpg"  => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".png"  => "image/png",
                    ".gif"  => "image/gif",
                    ".bmp"  => "image/bmp",
                    ".svg"  => "image/svg+xml",
                    ".webp" => "image/webp",
                    ".txt"  => "text/plain",
                    ".csv"  => "text/csv",
                    ".json" => "application/json",
                    ".xml"  => "application/xml",
                    ".zip"  => "application/zip",
                    ".rar"  => "application/vnd.rar",
                    ".7z"   => "application/x-7z-compressed",
                    ".doc"  => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".xls"  => "application/vnd.ms-excel",
                    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    ".ppt"  => "application/vnd.ms-powerpoint",
                    ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                    ".mp3"  => "audio/mpeg",
                    ".wav"  => "audio/wav",
                    ".mp4"  => "video/mp4",
                    ".mov"  => "video/quicktime",
                    ".avi"  => "video/x-msvideo",
                    ".wmv"  => "video/x-ms-wmv",
                    ".mkv"  => "video/x-matroska",
                    ".html" => "text/html",
                    ".htm"  => "text/html",
                    ".css"  => "text/css",
                    ".js"   => "application/javascript",
                    _       => "application/octet-stream"
                };
            }
        }
    }
}