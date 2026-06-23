
using API.Seguridad.Application.Core;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Seguridad.Application.Anexos.Commands
{
    public static class CreateAnexo
    {
        public class Command : IRequest<Result<int>>
        {
            public int LinkId { get; set; }
            public TiposLink TipoLink { get; set; } = TiposLink.Otro;
            public TiposAnexo TipoAnexo { get; set; } = TiposAnexo.Otro;
            public TiposArchivo TipoArchivo { get; set; } = TiposArchivo.Otro;
            public string? NombreArchivo { get; set; } 
            public string? Extension { get; set; } // Optional, if used for display
            public IFormFile? ArchivoBlob { get; set; }
        }
    }

    public static class UpdateAnexo
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int? Id { get; set; }
            public int LinkId { get; set; }
            public TiposLink TipoLink { get; set; } = TiposLink.Otro;
            public TiposAnexo TipoAnexo { get; set; } = TiposAnexo.Otro;
            public TiposArchivo TipoArchivo { get; set; } = TiposArchivo.Otro;
            public string? NombreArchivo { get; set; }
            public string? Extension { get; set; }
            public IFormFile? ArchivoBlob { get; set; }
        }
    }

    public static class DeleteAnexo
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
        }
    }

    public class CreateAnexoHandler(
        UnidadEmpleoDBContextFactoryInterface factory,
        IMapper mapper,
        IAntivirusScanner antivirusScanner,
        IConfiguration configuration, 
        ILogger<CreateAnexoHandler> _logger
    ) : IRequestHandler<CreateAnexo.Command, Result<int>>
    {
        public async Task<Result<int>> Handle(CreateAnexo.Command request, CancellationToken cancellationToken)
        {
            if (request.ArchivoBlob == null || request.ArchivoBlob.Length == 0)
                return Result<int>.Failure("Archivo no proporcionado", 400);

            var maxFileSizeMB = configuration.GetValue<int>("Anexos:MaxFileSizeMB", 10);
            long maxFileSize = maxFileSizeMB * 1024 * 1024;
            if (request.ArchivoBlob.Length > maxFileSize)
                return Result<int>.Failure($"El archivo excede el tamaño permitido ({maxFileSizeMB}MB)", 400);

            var allowedTypes = configuration.GetSection("Anexos:AllowedTypes").Get<string[]>() ?? ["application/pdf", "image/jpeg"];
            if (!allowedTypes.Contains(request.ArchivoBlob.ContentType))
                return Result<int>.Failure("Tipo de archivo no permitido", 400);

            await using var db = await factory.CreateAsync();
            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            Anexo anexo;

            try
            {
                using var ms = new MemoryStream();
                await request.ArchivoBlob.CopyToAsync(ms, cancellationToken);
                ms.Position = 0;

                // Leer flag de escaneo antivirus
                var antivirusEnabled = configuration.GetValue<bool>("Anexos:AntivirusEnabled", false);
                if (antivirusEnabled)
                {
                    var isClean = await antivirusScanner.ScanAsync(ms, cancellationToken);
                    if (!isClean)
                        return Result<int>.Failure("El archivo contiene un virus", 400);
                }

                anexo = mapper.Map<Anexo>(request);
                anexo.Blob = ms.ToArray();

                db.Anexos.Add(anexo);
                await db.SaveChangesAsync(cancellationToken);

                switch(request.TipoLink)
                {
                    case TiposLink.Paciente:
                        /*
                        var paciente = await db.Pacientes.FindAsync([request.LinkId], cancellationToken);   
                        if (paciente != null)
                        {
                            paciente.FechaUltimaActualizacion = DateTime.UtcNow;
                            paciente.FotoId = anexo.Id; // Ejemplo de asociación
                            await db.SaveChangesAsync(cancellationToken);
                        }
                        */
                        break;
                    default:
                        break;
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {                
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error al crear el anexo");
                return Result<int>.Failure($"Error al crear el anexo: {ex.Message}", 500);
            }

            return Result<int>.Success(anexo.Id);
        }
    }

    public class UpdateAnexoHandler(
        UnidadEmpleoDBContextFactoryInterface factory,
        IMapper mapper,
        IAntivirusScanner antivirusScanner,
        IConfiguration configuration 
    ) : IRequestHandler<UpdateAnexo.Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(UpdateAnexo.Command request, CancellationToken cancellationToken)
        {
            if (request.Id == null)
                return Result<Unit>.Failure("Id del anexo no proporcionado", 400);

            if (request.ArchivoBlob == null || request.ArchivoBlob.Length == 0)
                return Result<Unit>.Failure("Archivo no proporcionado", 400);

            var maxFileSizeMB = configuration.GetValue<int>("Anexos:MaxFileSizeMB", 10);
            long maxFileSize = maxFileSizeMB * 1024 * 1024;
            if (request.ArchivoBlob.Length > maxFileSize)
                return Result<Unit>.Failure($"El archivo excede el tamaño permitido ({maxFileSizeMB}MB)", 400);

            var allowedTypes = configuration.GetSection("Anexos:AllowedTypes").Get<string[]>() ?? ["application/pdf", "image/jpeg"];
            if (!allowedTypes.Contains(request.ArchivoBlob.ContentType))
                return Result<Unit>.Failure("Tipo de archivo no permitido", 400);

            await using var db = await factory.CreateAsync();
            var anexo = await db.Anexos.FindAsync([request.Id], cancellationToken);

            if (anexo == null)
                return Result<Unit>.Failure("Anexo no encontrado", 404);

            mapper.Map(request, anexo);

            using var ms = new MemoryStream();
            await request.ArchivoBlob.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;

            // Leer flag de escaneo antivirus
            var antivirusEnabled = configuration.GetValue<bool>("Anexos:AntivirusEnabled", false);
            if (antivirusEnabled)
            {
                var isClean = await antivirusScanner.ScanAsync(ms, cancellationToken);
                if (!isClean)
                    return Result<Unit>.Failure("El archivo contiene un virus", 400);
            }

            anexo.Blob = ms.ToArray();

            await db.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }

    public class DeleteAnexoHandler(
        UnidadEmpleoDBContextFactoryInterface factory
    ) : IRequestHandler<DeleteAnexo.Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(DeleteAnexo.Command request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                return Result<Unit>.Failure("Id del anexo no proporcionado", 400);

            await using var db = await factory.CreateAsync();
            var anexo = await db.Anexos.FindAsync([request.Id], cancellationToken);
            if (anexo == null)
                return Result<Unit>.Failure("Anexo no encontrado", 404);

            db.Anexos.Remove(anexo);
            await db.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}