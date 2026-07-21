using API.Common.Domain;
using API.Seguridad.Domain.Audit;
using API.UnidadEmpleo.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using static API.Seguridad.Infrastructure.Audit.Enumerations.Audits;

namespace API.UnidadEmpleo.Persistence
{
    public class UnidadEmpleoDbContext : DbContext
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        // nuestras clases db
        public DbSet<Region> Region { get; set; }
        public DbSet<Cuerpo> Corporacion { get; set; }
        public DbSet<Solicitud> Solicitud { get; set; }
        public DbSet<Referencia> Referencia { get; set; }
        public DbSet<CartaCompromiso> CartaCompromiso { get; set; }
        public DbSet<Evaluacion> Evaluacion { get; set; }
        public DbSet<Aspirante> Aspirante { get; set; }
        public DbSet<EntityChangeLog> EntityChangeLogs { get; set; }
        public DbSet<Anexo> Anexos { get; set; }

        // Constructor usado por EF / migraciones (sin HttpContext)  se queda intacto
        public UnidadEmpleoDbContext(DbContextOptions<UnidadEmpleoDbContext> options)
            : base(options){}

        // Constructor para inyección normal con auditoría
        public UnidadEmpleoDbContext(DbContextOptions<UnidadEmpleoDbContext> options,
            IHttpContextAccessor httpContextAccessor)
            : base(options){  
            _httpContextAccessor = httpContextAccessor;        
        }

        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=UnidadEmpleo;Integrated Security=True;TrustServerCertificate=True")
                    .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
            }
            else
            {
                // Ensure the warning is ignored even when options are provided externally
                optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
            }
        }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // SETTING DB RELATIONS AND CARACTERISTICS

            builder.Entity<Region>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            builder.Entity<Cuerpo>(entity =>
            {
                entity.HasKey(e => e.Id);
                
            });

            builder.Entity<Cuerpo>()
                .HasMany(e => e.Regiones)
                .WithMany(e => e.Cuerpos);

            builder.Entity<Aspirante>(entity =>
            {
                entity.HasKey(e => e.Curp);
                entity.Property(e => e.Nombre).IsRequired();
                entity.Property(e => e.Fecha_Nacimiento).IsRequired();
                
                entity.HasMany(a => a.Solicitudes)
                      .WithOne(s => s.Aspirante)
                      .HasForeignKey(s => s.Curp);

                entity.HasOne(s => s.CuerpoCaptura)
                       .WithMany()
                       .HasForeignKey(s => s.IdCuerpoCaptura)
                       .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Solicitud>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(s => s.Aspirante)
                       .WithMany(a => a.Solicitudes)
                       .HasForeignKey(s => s.Curp);

                entity.HasOne(s => s.Corporacion)
                      .WithMany()
                      .HasForeignKey(s => s.CorporacionId);

                entity.HasOne(s => s.Region)
                      .WithMany()
                      .HasForeignKey(s => s.RegionId);

                entity.HasMany(s => s.Evaluaciones)
                      .WithOne(ev => ev.Solicitud)
                      .HasForeignKey(ev => ev.IdSoliciud);

                entity.HasMany(s => s.Referencias)
                      .WithOne(ev => ev.Solicitud)
                      .HasForeignKey(ev => ev.IdSoliciud);

                entity.HasMany(s => s.CartasCompromiso)
                      .WithOne(ev => ev.Solicitud)
                      .HasForeignKey(ev => ev.IdSoliciud);

            });

            builder.Entity<Referencia>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Id).ValueGeneratedOnAdd();
                entity.HasOne(r => r.Solicitud)
                        .WithMany(s => s.Referencias)
                        .HasForeignKey(r => r.IdSoliciud);
            });

            builder.Entity<Evaluacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(r => r.Id).ValueGeneratedOnAdd();
                entity.HasOne(e => e.Solicitud)
                        .WithMany(s => s.Evaluaciones)
                        .HasForeignKey(e => e.IdSoliciud);
            });

            builder.Entity<CartaCompromiso>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(e => e.Solicitud)
                        .WithMany(s => s.CartasCompromiso)
                        .HasForeignKey(e => e.IdSoliciud);
            });

            builder.Entity<Anexo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
            
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            
            var auditEntries = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);

            if (result == 0 && auditEntries.Count == 0) //Significa que no hay cambios y por ende no hizo un guardado en la bd.
                return 1;

            if (result == 0 || auditEntries.Count == 0)
                return result; 

            EntityChangeLogs.AddRange(auditEntries);
            await base.SaveChangesAsync(cancellationToken);
            return result;
        }

        public override int SaveChanges()
        {
            
            var auditEntries = OnBeforeSaveChanges();
            var result = base.SaveChanges();

            if (result == 0 || auditEntries.Count == 0)
                return result;

            EntityChangeLogs.AddRange(auditEntries);
            base.SaveChanges();
            return result;
        }


        private List<EntityChangeLog> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();

            var userId = API.Seguridad.Infrastructure.AuthContext.GetUserId(_httpContextAccessor?.HttpContext?.User) ?? "System";
            var userName = API.Seguridad.Infrastructure.AuthContext.GetUserName(_httpContextAccessor?.HttpContext?.User) ?? "System";

            var auditEntries = new List<EntityChangeLog>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is EntityChangeLog ||
                    entry.Entity is not IAuditable ||
                    entry.State == EntityState.Detached ||
                    entry.State == EntityState.Unchanged)
                {
                    continue;
                }

                var entityName = entry.Entity.GetType().Name;
                var entityId = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString() ?? "";

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
                    continue;

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
