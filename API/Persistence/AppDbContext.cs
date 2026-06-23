
using API.Common.Domain;
using API.Seguridad.Domain.Audit;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.Domain.Ubicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using System.Reflection.Emit;
using static API.Seguridad.Infrastructure.Audit.Enumerations.Audits;

namespace API.Persistence
{
    public class AppDbContext : IdentityDbContext<AppUserIdentity, AppIdentityRole, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, 
            IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public required DbSet<Usuario> Usuarios { get; set; }
        public required DbSet<Grupo> Grupos { get; set; }
        public required DbSet<Proceso> Procesos { get; set; }
        public required DbSet<RolProceso> RolesProcesos { get; set; }
        public required DbSet<Permiso> Permisos { get; set; }
        public required DbSet<Corporacion> Corporaciones { get; set; }
        public required DbSet<UsuarioCorporacion> UsuarioCorporaciones { get; set; }
        public required DbSet<AuditLog> AuditLogs { get; set; }
        public required DbSet<EntityChangeLog> EntityChangeLogs { get; set; }
        public required DbSet<LogEntry> Logs { get; set; }
        public required DbSet<EntidadFederativa> EntidadFederativas { get; set; }
        public required DbSet<Sistema> Sistemas { get; set; }
        public required DbSet<BaseDatos> BasesDatos { get; set; }
        public required DbSet<CorporacionSistemaBD> CorporacionSistemaBDs { get; set; }
        public required DbSet<EstadoCivil> EstadosCiviles { get; set; }
        public required DbSet<NivelEducativo> NivelesEducativos { get; set; }
        public required DbSet<CodigoPostal> CodigoPostal { get; set; }
        public required DbSet<TipoRolesNoUtil> TipoRoles { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CodigoPostal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
            });
            builder.Entity<TipoRolesNoUtil>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            // Configuración de la relación uno a uno entre Usuario y AppUserIdentity
            builder.Entity<Usuario>()
                .HasOne(u => u.AppUserIdentity)
                .WithOne(a => a.Usuario)
                .HasForeignKey<Usuario>(u => u.AppUserIdentityId);

            builder.Entity<Proceso>()
                .HasOne(p => p.ProcesoPadre)
                .WithMany(p => p.Subprocesos)
                .HasForeignKey(p => p.ProcesoPadreId)
                .OnDelete(DeleteBehavior.Restrict); // Evita eliminación en cascada

            builder.Entity<Proceso>().HasOne(s => s.Sistema)
                       .WithMany()
                       .HasForeignKey(s => s.SistemaId);

            builder.Entity<RolProceso>()
                .HasKey(rp => new { rp.RolId, rp.ProcesoId });

            builder.Entity<Permiso>(entity =>
            {
                entity.Property(p => p.Acceso)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.HasIndex(p => new { p.UsuarioId, p.RolId, p.ProcesoId })
                      .IsUnique();
            });

            builder.Entity<Corporacion>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Nombre)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasIndex(c => c.Nombre)
                      .IsUnique();
            });

            // Configuración de la relación muchos a muchos entre Usuario y Corporacion
            builder.Entity<UsuarioCorporacion>()
                .HasKey(uc => new { uc.UsuarioId, uc.CorporacionId });

            builder.Entity<UsuarioCorporacion>()
                .HasOne(uc => uc.Usuario)
                .WithMany(u => u.UsuarioCorporaciones)
                .HasForeignKey(uc => uc.UsuarioId);

            builder.Entity<UsuarioCorporacion>()
                .HasOne(uc => uc.Corporacion)
                .WithMany(c => c.UsuarioCorporaciones)
                .HasForeignKey(uc => uc.CorporacionId);

            builder.Entity<Sistema>(entity =>
            {
                entity.Property(s => s.Id)
                .ValueGeneratedNever();
            });

            builder.Entity<BaseDatos>(entity =>
            {
                entity.Property(s => s.Id)
                .ValueGeneratedNever();
            });

            builder.Entity<CorporacionSistemaBD>(entity =>
            {
                entity.HasKey(cs => new { cs.CorporacionId, cs.SistemaId, cs.BaseDatosId });

                entity.HasOne(cs => cs.Corporacion)
                      .WithMany()
                      .HasForeignKey(cs => cs.CorporacionId);
                entity.HasOne(cs => cs.Sistema)
                      .WithMany()
                      .HasForeignKey(cs => cs.SistemaId);
                entity.HasOne(cs => cs.BaseDatos)
                      .WithMany()
                      .HasForeignKey(cs => cs.BaseDatosId);
            });

            builder.Entity<NivelEducativo>(entity =>
            {
                entity.Property(s => s.Id)
                .ValueGeneratedNever();
            });

            builder.Entity<EstadoCivil>(entity =>
            {
                entity.Property(s => s.Id)
                .ValueGeneratedNever();
            });

            builder.Entity<UserRefreshToken>(entity =>
            {
                entity.HasIndex(r => new { r.UserId, r.TokenHash }).IsUnique();
                entity.Property(r => r.TokenHash).IsRequired();
            });


        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = OnBeforeSaveChanges();
            try { 
                var result = await base.SaveChangesAsync(cancellationToken);

                if (result == 0)
                {
                    return result;
                }

                if (auditEntries.Any())
                {
                    EntityChangeLogs.AddRange(auditEntries);
                    await base.SaveChangesAsync(cancellationToken);
                }

                return result;
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        private List<EntityChangeLog> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value ?? "System";
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";

            var auditEntries = new List<EntityChangeLog>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog ||
                    entry.Entity is EntityChangeLog ||
                    entry.Entity is not IAuditable || 
                    entry.State == EntityState.Detached ||
                    entry.State == EntityState.Unchanged)
                {
                    continue;
                }

                var entityName = entry.Entity.GetType().Name;
                var entityId = entry
                    .Properties
                    .FirstOrDefault(p => p.Metadata.IsPrimaryKey())?
                    .CurrentValue?
                    .ToString() ?? "";

                var changeType = entry.State switch
                {
                    EntityState.Added => EntityActionType.Created,
                    EntityState.Modified => EntityActionType.Updated,
                    EntityState.Deleted => EntityActionType.Deleted,
                    _ => EntityActionType.None
                };

                var changes = new Dictionary<string, object?>();
                foreach (var prop in entry.Properties)
                {
                    if (prop.IsTemporary ||
                        prop.Metadata.IsPrimaryKey() ||
                        prop.Metadata.Name.Equals("FechaUltimaActualizacion", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            changes[prop.Metadata.Name] = new { New = prop.CurrentValue };
                            break;
                        case EntityState.Deleted:
                            changes[prop.Metadata.Name] = new { Old = prop.OriginalValue };
                            break;
                        case EntityState.Modified when prop.IsModified:
                            changes[prop.Metadata.Name] = new { Old = prop.OriginalValue, New = prop.CurrentValue };
                            break;
                    }
                }

                if (changes.Count == 0)
                {
                    continue;
                }

                auditEntries.Add(new EntityChangeLog
                {
                    UserId = userId,
                    UserName = userName,
                    ChangeType = changeType,
                    EntityName = entityName,
                    EntityId = entityId,
                    Changes = JsonConvert.SerializeObject(changes)
                });
            }

            return auditEntries;
        }
    }
}