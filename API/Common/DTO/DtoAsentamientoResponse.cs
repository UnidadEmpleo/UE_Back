using API.Common.Domain;
namespace API.Common.DTO
{
    public class DtoAsentamientoResponse
    {
        public int c_codigo { get; set; } = 0;
        /*
        public List<CodigoPostal> municipio { get; set; } = new List<CodigoPostal>();
        public List<CodigoPostal> estado { get; set; } = new List<CodigoPostal>();
        public List<CodigoPostal> tipoAsentamiento { get; set; } = new List<CodigoPostal>();
        public List<CodigoPostal> asentamiento { get; set; } = new List<CodigoPostal>();
        */

        public List<DtoMunicipio> municipio { get; set; } = new List<DtoMunicipio>();
        public List<DtoEstado> estado { get; set; } = new List<DtoEstado>();
        public List<DtoTipoAsentamiento> tipoAsentamiento { get; set; } = new List<DtoTipoAsentamiento>();
        public List<CodigoPostal> asentamiento { get; set; } = new List<CodigoPostal>();
        

    }

    public class Asentamiento
    {
        public long Id { get; set; }
        public int c_mnpio { get; set; }
        public int c_estado { get; set; }
        public int c_tipo_asenta { get; set; }
        public int c_codigo { get; set; }
        public string d_mnpio { get; set; }
        public string d_estado { get; set; }
        public string d_tipo_asenta { get; set; }
        public string d_asenta { get; set; }
    }

    public class DtoTipoAsentamiento
    {
        public int c_tipo_asenta { get; set; }
        public string d_tipo_asenta { get; set; }
    }
    public class DtoMunicipio
    {
        public int c_mnpio { get; set; }
        public string d_mnpio { get; set; }
    }

    public class DtoEstado
    {
        public int c_estado { get; set; }
        public string d_estado { get; set; }
    }

}
