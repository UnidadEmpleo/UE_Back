using API.Common.Domain;
using API.Persistence;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;



namespace API.UnidadEmpleo.Services
{
    public interface ILists
    {
        Task<List<Cuerpo>> GetCorporacionListAsync();
        Task<List<Region>> GetRegionsAsync();
    }

    public class ListsServices : ILists
    {
        private readonly UnidadEmpleoDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly UnidadEmpleoDBContextFactoryInterface _factory;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(6);

        public ListsServices(UnidadEmpleoDbContext context, IMemoryCache cache, UnidadEmpleoDBContextFactoryInterface factory, IMapper mapper)
        {
            _context = context;
            _cache = cache;
            _factory = factory;
            _mapper = mapper;
        }


        async Task<List<Cuerpo>> ILists.GetCorporacionListAsync()
        {
            return await _cache.GetOrCreateAsync("Corporaciones", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
                
                return await _context.Corporacion.AsNoTracking<Cuerpo>().ToListAsync();// db.Corporacion.AsNoTracking<Corporacion>().ToListAsync();
            });
        }

        async Task<List<Region>> ILists.GetRegionsAsync()
        {
            return await _cache.GetOrCreateAsync("Regiones", async entry => {
                entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
                return await _context.Region.AsNoTracking<Region>().ToListAsync();
            }) ?? new List<Region>();
        }
    }

}
