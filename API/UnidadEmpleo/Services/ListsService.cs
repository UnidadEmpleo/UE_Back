using API.Common.Domain;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace API.UnidadEmpleo.Services
{
    public class ListsService 
    {
        private readonly UnidadEmpleoDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly UnidadEmpleoDBContextFactoryInterface _factory;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(6);

        public async Task<List<Cuerpo>> GetCorporacionListAsync()
        {
            return await _cache.GetOrCreateAsync("Corporaciones", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
                return await _context.Corporacion.AsNoTracking<Cuerpo>().ToListAsync();
            })?? new List<Cuerpo>();
        }

        public async Task<List<Region>> GetRegionsAsync()
        {
            return await _cache.GetOrCreateAsync("Regiones", async entry => {
                entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
                return await _context.Region.AsNoTracking<Region>().ToListAsync();
            })?? new List<Region>();
        }

        
    }
}
