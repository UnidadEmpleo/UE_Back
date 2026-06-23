using API.Common.Domain;
using API.Common.DTO;
using API.Persistence;
using API.Seguridad.Application.Core;
using API.UnidadEmpleo.DTO;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace API.Common.App
{
    public class GetDatosAsentamientoByCP
    {

        /*
         --select d_codigo,d_asenta, d_tipo_asenta, D_mnpio, d_estado, c_estado, c_tipo_asenta, c_mnpio from todo where d_codigo = 1130
         --select distinct [c_estado], [d_estado] from (select d_codigo,d_asenta, d_tipo_asenta, D_mnpio, d_estado, c_estado, c_tipo_asenta, c_mnpio from todo where d_codigo = 1130) as x --estados
         --select distinct c_estado, [c_mnpio], D_mnpio from (select d_codigo,d_asenta, d_tipo_asenta, D_mnpio, d_estado, c_estado, c_tipo_asenta, c_mnpio from todo where d_codigo = 1130) as x -- municipios
         --select distinct (c_tipo_asenta), d_tipo_asenta  from (select d_codigo,d_asenta, d_tipo_asenta, D_mnpio, d_estado, c_estado, c_tipo_asenta, c_mnpio from todo where d_codigo = 1130) as x --tipos de asentamientos
         --select distinct (d_asenta)  from (select d_codigo,d_asenta, d_tipo_asenta, D_mnpio, d_estado, c_estado, c_tipo_asenta, c_mnpio from todo where d_codigo = 1130) as x -- asentamientos
        select * from todo where c_estado = 3 and c_mnpio = 3 and d_asenta like '%ara%'

         */

        public class Query : IRequest<Result<DtoAsentamientoResponse>>
        {
            public int c_codigo { get; set; }
        }

        public class Handler(AppDbContext dbContext, IMapper mapper) : IRequestHandler<Query, Result<DtoAsentamientoResponse>>
        {
            public async Task<Result<DtoAsentamientoResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                DtoAsentamientoResponse dto = new DtoAsentamientoResponse();

                List<CodigoPostal>? items = await dbContext.Set<CodigoPostal>()
                    .AsNoTracking()
                    .Where(x => x.c_codigo == request.c_codigo)
                    .ToListAsync(cancellationToken);

                if (items == null)
                    return Result<DtoAsentamientoResponse>.Failure("No se encontraron datos", 404);

                IEnumerable<CodigoPostal> distinctEstado = items.DistinctBy(note => note.c_estado);
                foreach (CodigoPostal p in distinctEstado)
                    dto.estado.Add(new DtoEstado { c_estado = p.c_estado, d_estado = p.d_estado });
                
                IEnumerable<CodigoPostal> distinctMunicipio = items.DistinctBy(note => note.c_mnpio);
                foreach (CodigoPostal p in distinctMunicipio)
                    dto.municipio.Add(new DtoMunicipio { c_mnpio = p.c_mnpio, d_mnpio = p.d_mnpio });

                IEnumerable<CodigoPostal> distinctTipoAsentamiento = items.DistinctBy(note => note.c_tipo_asenta);
                foreach (CodigoPostal p in distinctTipoAsentamiento)
                    dto.tipoAsentamiento.Add(new DtoTipoAsentamiento { c_tipo_asenta = p.c_tipo_asenta, d_tipo_asenta = p.d_tipo_asenta });

                IEnumerable<CodigoPostal> distinctAsentamiento = items.DistinctBy(note => note.c_tipo_asenta);
                foreach (CodigoPostal p in distinctAsentamiento)
                    dto.asentamiento.Add(p);// new Asentamiento { c_tipo_asenta = p.c_tipo_asenta, c_asenta = p.d_asenta, d_asenta = p.d_asenta });

                dto.c_codigo = request.c_codigo;

                return Result<DtoAsentamientoResponse>.Success(dto);
            }
        }
    }

}
