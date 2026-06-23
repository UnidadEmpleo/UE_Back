using API.Persistence;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.Infrastructure;
using API.UnidadEmpleo.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace API.UnidadEmpleo.Persistence
{
    public class UnidadEmpleoDbContextDesignFactory : IDesignTimeDbContextFactory<UnidadEmpleoDbContext>
    {
        public UnidadEmpleoDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UnidadEmpleoDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=UnidadEmpleo;Integrated Security=True;TrustServerCertificate=True");
            return new UnidadEmpleoDbContext(optionsBuilder.Options);
        }
    }

    public class UnidadEmpleoDbContextFactory : UnidadEmpleoDBContextFactoryInterface
    {
            private readonly AppDbContext _accessDbContext; 
            private readonly UnidadEmpleoDbContext _appDbContext;
            private readonly ICorporacionContextAccessor _accessor;
            private readonly IMemoryCache _cache;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public UnidadEmpleoDbContextFactory(
                AppDbContext accessDbContext,
                UnidadEmpleoDbContext appDbContext,
                ICorporacionContextAccessor accessor,
                IMemoryCache cache,
                IHttpContextAccessor httpContextAccessor)
            {
                _accessDbContext = accessDbContext;
                _appDbContext = appDbContext;
                _accessor = accessor;
                _cache = cache;
                _httpContextAccessor = httpContextAccessor;
            
            }

        public async Task<UnidadEmpleoDbContext> CreateAsync()
        {

            var corporacionId = "";// _accessor.CorporacionId ?? throw new Exception("CorporacionId no proporcionado.");
            //var sistemaId = "";//_accessor.SistemaId ?? throw new Exception("SistemaId no proporcionado.");

            
            var dbOptions = await _cache.GetOrCreateAsync($"", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                var corpSistemaBD = await _accessDbContext
                .CorporacionSistemaBDs
                .Include(c => c.BaseDatos)
                .Include(c => c.Corporacion)
                .FirstOrDefaultAsync(c => c.Corporacion.Nombre == "Unidad de Empleo" &&
                    c.SistemaId == 3) ?? throw new Exception("Corporación no encontrada.");

                return corpSistemaBD;
            });

            var optionsBuilder = new DbContextOptionsBuilder<UnidadEmpleoDbContext>();
            optionsBuilder.UseSqlServer(BuildConnectionString(dbOptions));

            var dbContext = new UnidadEmpleoDbContext(optionsBuilder.Options, _httpContextAccessor);

            var conn = dbContext.Database.GetDbConnection().ConnectionString;
            Console.WriteLine(conn);


            return dbContext;
        }

        private string BuildConnectionString(CorporacionSistemaBD corporacion)
            {
                var connString = corporacion.BaseDatos?.ConnectionString
                    ?? throw new Exception("La plantilla de cadena de conexión no está definida.");

                return connString;
            }
        }

    
}
