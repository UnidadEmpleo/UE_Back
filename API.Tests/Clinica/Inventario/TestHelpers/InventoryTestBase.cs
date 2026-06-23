using System.Linq;
using System.Threading.Tasks;

using API.Persistence;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Tests.Clinica.Inventario.TestHelpers
{
    public abstract class InventoryTestBase
    {
        protected IMapper CreateMapper()
        {
            var cfg = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new API.Application.Core.MappingProfiles());
            });
            return cfg.CreateMapper();
        }

        protected CorporacionDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<CorporacionDbContext>()
                .UseInMemoryDatabase(dbName)
                .EnableSensitiveDataLogging()
                .Options;

            var ctx = new CorporacionDbContext(options);
            return ctx;
        }

        protected async Task SeedBaseCatalogsAsync(CorporacionDbContext ctx)
        {
            if (!ctx.Set<Farmacia>().Any())
            {
                ctx.Set<Farmacia>().Add(new Farmacia { Id = 1, Nombre = "Farmacia Central", Activo = true });
            }

            if (!ctx.Set<MedicamentoTipoDocumento>().Any())
            {
                ctx.Set<MedicamentoTipoDocumento>().Add(new MedicamentoTipoDocumento { Id = 1, Nombre = "Factura", Activo = true });
            }

            await ctx.SaveChangesAsync();
        }

        protected sealed class FakeCorporacionDbContextFactory : ICorporacionDbContextFactory
        {
            private readonly CorporacionDbContext _ctx;
            public FakeCorporacionDbContextFactory(CorporacionDbContext ctx) => _ctx = ctx;
            public Task<CorporacionDbContext> CreateAsync() => Task.FromResult(_ctx);
        }
    }
}
