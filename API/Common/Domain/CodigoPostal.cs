namespace API.Common.Domain
{
    public class CodigoPostal
    {
        public long Id { get; set; }
        public int c_mnpio { get; set; }
        public int c_estado { get; set; }
        public int c_tipo_asenta { get; set; }
        public int c_codigo { get; set; }
        public string d_mnpio {  get; set; }
        public string d_estado { get; set; }
        public string d_tipo_asenta { get; set; }
        public string d_asenta { get; set; }
        public float latitud { get; set; }
        public float longitud { get; set; }

    }

    // AGREGAR LA PARTE DE GOOGLE MAPS AL FRONT END PARA SACAR LA UBICACIÓN DE LA DIRECCIÓN.

}
