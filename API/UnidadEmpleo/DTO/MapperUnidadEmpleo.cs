
using API.UnidadEmpleo.Domain;
using AutoMapper;

namespace API.UnidadEmpleo.DTO
{
    public class MapperUnidadEmpleo : Profile
    {
        public MapperUnidadEmpleo()
        {
            CreateMap<Aspirante, AspiranteDto>();
        }
    }
}
