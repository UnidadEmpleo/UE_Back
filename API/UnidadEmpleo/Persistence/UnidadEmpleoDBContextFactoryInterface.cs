namespace API.UnidadEmpleo.Persistence
{
    public interface UnidadEmpleoDBContextFactoryInterface
    {
        Task<UnidadEmpleoDbContext> CreateAsync();
    }
}
