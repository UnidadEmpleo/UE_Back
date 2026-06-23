using API.Common.Domain;
using API.Seguridad.Application.Anexos.Commands;
using API.Seguridad.Application.Seguridad.Procesos.Commands;
using API.Seguridad.Domain.Audit;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.DTOs.Anexos;
using API.Seguridad.DTOs.AuditLogs;
using API.Seguridad.DTOs.Seguridad;
using API.UnidadEmpleo.Application.AspiranteApp;
using API.UnidadEmpleo.Application.CartaCompromisoApp;
using API.UnidadEmpleo.Application.Evln;
using API.UnidadEmpleo.Application.ReferenciaApp;
using API.UnidadEmpleo.Application.SolicitudSvs;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.DTO;
using AutoMapper;

namespace API.Seguridad.Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            
            CreateMap<Grupo, GroupDto>();

            CreateMap<Usuario, UserDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AppUserIdentity.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.AppUserIdentity.UserName))
                .ForMember(dest => dest.Corporaciones, opt => opt.MapFrom(src => src.UsuarioCorporaciones.Select(s => s.CorporacionId).ToList()));

            CreateMap<AppIdentityRole, ApplicationRoleDto>();

            CreateMap<Domain.Seguridad.Corporacion, CorporacionDTO>();

            CreateMap<AuditLog, AuditLogDto>();
            CreateMap<LogEntry, LogEntryDto>();

            CreateMap<Permiso, PermisoDTO>();
            CreateMap<PermisoDTO, Permiso>();

            CreateMap<Grupo, Grupo>();
            CreateMap<Proceso, ProcesoDto>()
                .ForMember(dest => dest.Subprocesos, opt => opt.MapFrom(src => src.Subprocesos));

            // Commands -> Proceso
            CreateMap<CreateProceso.Command, Proceso>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaUltimaActualizacion, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.Ignore())
                .ForMember(dest => dest.Subprocesos, opt => opt.Ignore())
                .ForMember(dest => dest.ProcesoPadre, opt => opt.Ignore());

            CreateMap<UpdateProceso.Command, Proceso>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaUltimaActualizacion, opt => opt.Ignore())
                .ForMember(dest => dest.ProcesoPadreId, opt => opt.Ignore())
                .ForMember(dest => dest.Subprocesos, opt => opt.Ignore())
                .ForMember(dest => dest.ProcesoPadre, opt => opt.Ignore());

            
            // Command to Entity (ArchivoBlob handled in handler)
            CreateMap<CreateAnexo.Command, UnidadEmpleo.Domain.Anexo > ()
                .ForMember(dest => dest.Blob, opt => opt.Ignore());
            CreateMap<UpdateAnexo.Command, UnidadEmpleo.Domain.Anexo>()
                .ForMember(dest => dest.Blob, opt => opt.Ignore());

            CreateMap<UnidadEmpleo.Domain.Anexo, AnexoDTO>()
                .ForMember(dest => dest.BlobBase64, 
                    opt => opt.MapFrom(src => src.Blob != null ? Convert.ToBase64String(src.Blob) : null));

            // Unidad de Empleo
            CreateMap<AspiranteDto, Aspirante>();
            CreateMap<AspiranteCreate.Command, Aspirante>();
            CreateMap<AspiranteUpdate.Command, Aspirante>();
            
            CreateMap<SolicitudCreate.Command, Solicitud>();
            CreateMap<SolicitudUpdate.Command, Solicitud>();

            CreateMap<ReferenciaCreate.Command, Referencia>();
            CreateMap<ReferenciaUpdate.Command, Referencia>();

            CreateMap<CartaCompromisoCreate.Command, CartaCompromiso>();
            CreateMap<CartaCompromisoUpdate.Command, CartaCompromiso>();

            CreateMap<EvaluacionCreate.Command, Evaluacion>();
            CreateMap<EvaluacionUpdate.Command, Evaluacion>();

            CreateMap<Cuerpo, CuerpoDto>()
                .ForMember(dest => dest.Regiones, opt => opt.MapFrom(src => src.Regiones));

            CreateMap<Sistema, SistemaDto>();

        }
    }
}