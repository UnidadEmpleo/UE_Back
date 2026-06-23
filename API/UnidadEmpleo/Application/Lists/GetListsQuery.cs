using API.Common.Domain;
using API.Seguridad.Application.Core;
using API.UnidadEmpleo.Services;
using MediatR;

namespace API.UnidadEmpleo.Application.Lists
{

    public class GetCorporacionesQuery : IRequest<Result<List<Cuerpo>>> { }

    public class GetCorporacionesQueryHandler(ILists catalogoService) : IRequestHandler<GetCorporacionesQuery, Result<List<Cuerpo>>>
    {
        public async Task<Result<List<Cuerpo>>> Handle(GetCorporacionesQuery request, CancellationToken cancellationToken)
        {
            var corporaciones = await catalogoService.GetCorporacionListAsync() ;
            return Result<List<Cuerpo>>.Success(corporaciones);
        }
    }

    public class GetRegionesQuery : IRequest<Result<List<Region>>> { }

    public class GetRegionesQueryHandler(ILists catalogoService) : IRequestHandler<GetRegionesQuery, Result<List<Region>>>
    {
        public async Task<Result<List<Region>>> Handle(GetRegionesQuery request, CancellationToken cancellationToken)
        {
            var result = await catalogoService.GetRegionsAsync();
            return Result<List<Region>>.Success(result);
        }
    }

}


